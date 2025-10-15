using Launcher.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace LayoutEditor
{
    public class EditorForm : Form
    {
        private readonly MenuStrip _menuStrip;
        private readonly ToolStrip _toolStrip;
        private readonly ListBox _toolboxList;
        private readonly PropertyGrid _propertyGrid;
        private readonly Panel _canvasPanel;
        private readonly Panel _formSurface;
        private readonly Label _formTitleLabel;
        private readonly ContextMenuStrip _controlContextMenu;
        private readonly ToolStripMenuItem _deleteMenuItem;
        private readonly ToolStripMenuItem _bringToFrontMenuItem;
        private readonly ToolStripMenuItem _sendToBackMenuItem;
        private readonly ToolStripMenuItem _setBackgroundMenuItem;
        private readonly ToolStripMenuItem _clearBackgroundMenuItem;
        private readonly ToolStripMenuItem _setImageMenuItem;
        private readonly ToolStripMenuItem _clearImageMenuItem;
        private readonly ContextMenuStrip _formContextMenu;
        private readonly ToolStripMenuItem _setFormBackgroundMenuItem;
        private readonly ToolStripMenuItem _clearFormBackgroundMenuItem;
        private readonly FormPropertyView _formPropertyView;

        private readonly Dictionary<Control, ControlDesignMetadata> _controlMap = new Dictionary<Control, ControlDesignMetadata>();
        private readonly Dictionary<string, ControlDesignMetadata> _nameMap = new Dictionary<string, ControlDesignMetadata>(StringComparer.OrdinalIgnoreCase);

        private LayoutDefinition _currentLayout;
        private FormDefinition _formDefinition;
        private string _currentFilePath;
        private string _formBackgroundImagePath;
        private bool _isDirty;

        private Control _selectedControl;
        private ControlDesignMetadata _selectedMetadata;
        private ControlDesignMetadata _activeDragMetadata;
        private bool _isDragging;
        private bool _isResizing;
        private Point _dragOriginMouseScreen;
        private Point _dragOriginControl;
        private Size _dragOriginSize;
        private Point _nextControlLocation = new Point(24, 24);

        private static readonly string[] ToolboxItems = new[]
        {
            "Label",
            "Button",
            "PictureBox",
            "TextBox",
            "WebBrowser",
            "Panel"
        };

        public EditorForm()
        {
            Text = "Editor de Layout";
            Width = 1200;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 600);
            KeyPreview = true;

            _menuStrip = new MenuStrip();
            _toolStrip = new ToolStrip();
            _toolboxList = new ListBox();
            _propertyGrid = new PropertyGrid();
            _canvasPanel = new Panel();
            _formSurface = new DoubleBufferedPanel();
            _formTitleLabel = new Label();
            _controlContextMenu = new ContextMenuStrip();
            _formContextMenu = new ContextMenuStrip();
            _formPropertyView = new FormPropertyView(this);

            _deleteMenuItem = new ToolStripMenuItem("Excluir");
            _bringToFrontMenuItem = new ToolStripMenuItem("Trazer para frente");
            _sendToBackMenuItem = new ToolStripMenuItem("Enviar para trás");
            _setBackgroundMenuItem = new ToolStripMenuItem("Definir imagem de fundo...");
            _clearBackgroundMenuItem = new ToolStripMenuItem("Limpar imagem de fundo");
            _setImageMenuItem = new ToolStripMenuItem("Definir imagem...");
            _clearImageMenuItem = new ToolStripMenuItem("Limpar imagem");
            _setFormBackgroundMenuItem = new ToolStripMenuItem("Definir imagem de fundo...");
            _clearFormBackgroundMenuItem = new ToolStripMenuItem("Limpar imagem de fundo");

            InitializeUi();
            HookEvents();
            NewLayout();
        }

        private void InitializeUi()
        {
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("&Arquivo");
            fileMenu.DropDownItems.Add(CreateMenuItem("&Novo", Keys.Control | Keys.N, NewMenuItem_Click));
            fileMenu.DropDownItems.Add(CreateMenuItem("&Abrir...", Keys.Control | Keys.O, OpenMenuItem_Click));
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(CreateMenuItem("&Salvar", Keys.Control | Keys.S, SaveMenuItem_Click));
            fileMenu.DropDownItems.Add(CreateMenuItem("Salvar &como...", Keys.Control | Keys.Shift | Keys.S, SaveAsMenuItem_Click));
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(CreateMenuItem("&Sair", Keys.Alt | Keys.F4, ExitMenuItem_Click));

            ToolStripMenuItem editMenu = new ToolStripMenuItem("&Editar");
            editMenu.DropDownItems.Add(CreateMenuItem("E&xcluir", Keys.Delete, DeleteMenuItem_Click));
            editMenu.DropDownItems.Add(CreateMenuItem("Trazer para &frente", Keys.Control | Keys.Up, BringToFrontMenuItem_Click));
            editMenu.DropDownItems.Add(CreateMenuItem("Enviar para &trás", Keys.Control | Keys.Down, SendToBackMenuItem_Click));

            _menuStrip.Items.Add(fileMenu);
            _menuStrip.Items.Add(editMenu);
            MainMenuStrip = _menuStrip;

            _toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            _toolStrip.Items.Add(CreateToolStripButton("Novo", NewMenuItem_Click));
            _toolStrip.Items.Add(CreateToolStripButton("Abrir", OpenMenuItem_Click));
            _toolStrip.Items.Add(CreateToolStripButton("Salvar", SaveMenuItem_Click));
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(CreateToolStripButton("Excluir", DeleteMenuItem_Click));
            _toolStrip.Items.Add(CreateToolStripButton("Frente", BringToFrontMenuItem_Click));
            _toolStrip.Items.Add(CreateToolStripButton("Trás", SendToBackMenuItem_Click));

            SplitContainer mainSplit = new SplitContainer();
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.SplitterDistance = 230;
            mainSplit.Panel1MinSize = 200;

            Panel toolboxPanel = new Panel();
            toolboxPanel.Dock = DockStyle.Fill;
            toolboxPanel.Padding = new Padding(8);

            Label toolboxLabel = new Label();
            toolboxLabel.Text = "Componentes";
            toolboxLabel.Dock = DockStyle.Top;
            toolboxLabel.Height = 20;
            toolboxLabel.TextAlign = ContentAlignment.MiddleLeft;

            _toolboxList.Dock = DockStyle.Fill;
            _toolboxList.Items.AddRange(ToolboxItems);
            _toolboxList.BorderStyle = BorderStyle.FixedSingle;
            _toolboxList.Font = new Font(Font, FontStyle.Regular);
            _toolboxList.IntegralHeight = false;

            toolboxPanel.Controls.Add(_toolboxList);
            toolboxPanel.Controls.Add(toolboxLabel);
            mainSplit.Panel1.Controls.Add(toolboxPanel);

            SplitContainer rightSplit = new SplitContainer();
            rightSplit.Dock = DockStyle.Fill;
            rightSplit.Orientation = Orientation.Horizontal;
            rightSplit.SplitterDistance = 530;
            rightSplit.Panel1MinSize = 260;
            rightSplit.Panel2MinSize = 180;

            _canvasPanel.Dock = DockStyle.Fill;
            _canvasPanel.BackColor = Color.FromArgb(37, 37, 38);
            _canvasPanel.AutoScroll = true;

            _formTitleLabel.Text = "(sem título)";
            _formTitleLabel.AutoSize = false;
            _formTitleLabel.Size = new Size(240, 24);
            _formTitleLabel.ForeColor = Color.White;
            _formTitleLabel.BackColor = Color.FromArgb(52, 52, 52);
            _formTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _formTitleLabel.Location = new Point(24, 16);

            _formSurface.Location = new Point(24, 48);
            _formSurface.Size = new Size(990, 560);
            _formSurface.BackColor = Color.WhiteSmoke;
            _formSurface.BorderStyle = BorderStyle.FixedSingle;

            _canvasPanel.Controls.Add(_formSurface);
            _canvasPanel.Controls.Add(_formTitleLabel);

            rightSplit.Panel1.Controls.Add(_canvasPanel);

            _propertyGrid.Dock = DockStyle.Fill;
            _propertyGrid.HelpVisible = false;
            _propertyGrid.ToolbarVisible = false;

            rightSplit.Panel2.Controls.Add(_propertyGrid);

            mainSplit.Panel2.Controls.Add(rightSplit);

            Controls.Add(mainSplit);
            Controls.Add(_toolStrip);
            Controls.Add(_menuStrip);

            _controlContextMenu.Items.AddRange(new ToolStripItem[]
            {
                _deleteMenuItem,
                new ToolStripSeparator(),
                _bringToFrontMenuItem,
                _sendToBackMenuItem,
                new ToolStripSeparator(),
                _setBackgroundMenuItem,
                _clearBackgroundMenuItem,
                _setImageMenuItem,
                _clearImageMenuItem
            });

            _formContextMenu.Items.AddRange(new ToolStripItem[]
            {
                _setFormBackgroundMenuItem,
                _clearFormBackgroundMenuItem
            });

            _formSurface.ContextMenuStrip = _formContextMenu;
        }

        private ToolStripMenuItem CreateMenuItem(string text, Keys shortcutKeys, EventHandler handler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(text);
            menuItem.ShortcutKeys = shortcutKeys;
            menuItem.ShowShortcutKeys = true;
            menuItem.Click += handler;
            return menuItem;
        }

        private ToolStripButton CreateToolStripButton(string text, EventHandler handler)
        {
            ToolStripButton button = new ToolStripButton(text);
            button.DisplayStyle = ToolStripItemDisplayStyle.Text;
            button.Click += handler;
            return button;
        }

        private void HookEvents()
        {
            _toolboxList.MouseDoubleClick += ToolboxList_MouseDoubleClick;
            _toolboxList.KeyDown += ToolboxList_KeyDown;

            _formSurface.MouseDown += FormSurface_MouseDown;
            _formSurface.Paint += FormSurface_Paint;

            _propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;

            _controlContextMenu.Opening += ControlContextMenu_Opening;
            _formContextMenu.Opening += FormContextMenu_Opening;

            _deleteMenuItem.Click += DeleteMenuItem_Click;
            _bringToFrontMenuItem.Click += BringToFrontMenuItem_Click;
            _sendToBackMenuItem.Click += SendToBackMenuItem_Click;
            _setBackgroundMenuItem.Click += SetBackgroundMenuItem_Click;
            _clearBackgroundMenuItem.Click += ClearBackgroundMenuItem_Click;
            _setImageMenuItem.Click += SetImageMenuItem_Click;
            _clearImageMenuItem.Click += ClearImageMenuItem_Click;
            _setFormBackgroundMenuItem.Click += SetFormBackgroundMenuItem_Click;
            _clearFormBackgroundMenuItem.Click += ClearFormBackgroundMenuItem_Click;

            FormClosing += EditorForm_FormClosing;
            KeyDown += EditorForm_KeyDown;
        }

        private void NewLayout()
        {
            _currentLayout = new LayoutDefinition();
            _formDefinition = new FormDefinition();
            _currentLayout.Form = _formDefinition;
            _currentLayout.Controls = new Dictionary<string, ControlDefinition>();

            _formDefinition.ClientSize = new[] { 990, 560 };
            _formDefinition.Text = "MU Launcher";
            _formDefinition.Visible = true;
            _formDefinition.BackgroundImage = null;

            _formBackgroundImagePath = null;
            _currentFilePath = null;
            _isDirty = false;
            _nextControlLocation = new Point(24, 24);

            ClearDesignSurface();
            ApplyFormDefinition();
            SelectControl(null);
            UpdateWindowTitle();
        }

        private void ClearDesignSurface()
        {
            foreach (Control control in new List<Control>(_controlMap.Keys))
            {
                DetachDesignHandlers(control);
                control.Dispose();
            }

            _controlMap.Clear();
            _nameMap.Clear();
            _formSurface.Controls.Clear();
        }

        private void ApplyFormDefinition()
        {
            if (_formDefinition.ClientSize != null && _formDefinition.ClientSize.Length >= 2)
            {
                _formSurface.Size = new Size(_formDefinition.ClientSize[0], _formDefinition.ClientSize[1]);
            }
            else
            {
                _formSurface.Size = new Size(990, 560);
                _formDefinition.ClientSize = new[] { _formSurface.Width, _formSurface.Height };
            }

            SetFormTextInternal(_formDefinition.Text);
            ApplyFormBackgroundImage(_formDefinition.BackgroundImage, false);
            _canvasPanel.PerformLayout();
            _formSurface.Invalidate();
        }

        private void ToolboxList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelectedToolboxItem();
        }

        private void ToolboxList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddSelectedToolboxItem();
                e.Handled = true;
            }
        }

        private void AddSelectedToolboxItem()
        {
            if (_toolboxList.SelectedItem == null)
            {
                return;
            }

            string typeName = _toolboxList.SelectedItem.ToString();
            AddNewControl(typeName);
        }

        private void AddNewControl(string typeName)
        {
            Control control = CreateDesignControl(typeName);
            if (control == null)
            {
                MessageBox.Show(this, "Tipo de controle desconhecido: " + typeName, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string name = GenerateUniqueName(typeName);
            control.Name = name;
            control.Location = _nextControlLocation;

            UpdateNextControlLocation();

            ControlDesignMetadata metadata = new ControlDesignMetadata(control, name, typeName);
            metadata.Definition.Location = new[] { control.Left, control.Top };
            metadata.Definition.Size = new[] { control.Width, control.Height };
            metadata.Definition.Text = control.Text;
            metadata.Definition.Enabled = control.Enabled;
            metadata.Definition.Visible = control.Visible;

            _formSurface.Controls.Add(control);
            AttachDesignHandlers(control);
            _controlMap[control] = metadata;
            _nameMap[name] = metadata;

            SelectControl(control);
            MarkDirty();
        }

        private void UpdateNextControlLocation()
        {
            _nextControlLocation = new Point(_nextControlLocation.X + 28, _nextControlLocation.Y + 28);
            if (_nextControlLocation.X > _formSurface.Width - 100)
            {
                _nextControlLocation.X = 24;
            }
            if (_nextControlLocation.Y > _formSurface.Height - 80)
            {
                _nextControlLocation.Y = 24;
            }
        }

        private Control CreateDesignControl(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            string lower = typeName.Trim().ToLowerInvariant();
            switch (lower)
            {
                case "label":
                    Label label = new Label();
                    label.Size = new Size(140, 28);
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.Text = "Label";
                    label.BackColor = Color.FromArgb(240, 240, 240);
                    return label;
                case "button":
                    Button button = new Button();
                    button.Size = new Size(120, 36);
                    button.Text = "Botão";
                    return button;
                case "picturebox":
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Size = new Size(160, 120);
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;
                    pictureBox.BackColor = Color.Black;
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Text = string.Empty;
                    return pictureBox;
                case "textbox":
                    TextBox textBox = new TextBox();
                    textBox.Size = new Size(180, 24);
                    textBox.Text = "Texto";
                    return textBox;
                case "webbrowser":
                    Panel browserPlaceholder = new Panel();
                    browserPlaceholder.Size = new Size(320, 220);
                    browserPlaceholder.BorderStyle = BorderStyle.FixedSingle;
                    browserPlaceholder.BackColor = Color.White;
                    browserPlaceholder.Tag = "WebBrowser";
                    Label placeholderLabel = new Label();
                    placeholderLabel.Text = "WebBrowser";
                    placeholderLabel.Dock = DockStyle.Fill;
                    placeholderLabel.TextAlign = ContentAlignment.MiddleCenter;
                    placeholderLabel.ForeColor = Color.DimGray;
                    browserPlaceholder.Controls.Add(placeholderLabel);
                    return browserPlaceholder;
                case "panel":
                    Panel panel = new Panel();
                    panel.Size = new Size(220, 160);
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.BackColor = Color.FromArgb(220, 220, 220);
                    return panel;
                default:
                    Type type = Type.GetType(typeName, false, true);
                    if (type != null && typeof(Control).IsAssignableFrom(type))
                    {
                        try
                        {
                            object instance = Activator.CreateInstance(type);
                            return instance as Control;
                        }
                        catch
                        {
                        }
                    }
                    break;
            }

            return null;
        }

        private string GenerateUniqueName(string typeName)
        {
            string baseName = "control";
            string lower = typeName != null ? typeName.ToLowerInvariant() : string.Empty;
            if (lower.Contains("button"))
            {
                baseName = "button";
            }
            else if (lower.Contains("picture"))
            {
                baseName = "pictureBox";
            }
            else if (lower.Contains("label"))
            {
                baseName = "label";
            }
            else if (lower.Contains("text"))
            {
                baseName = "textBox";
            }
            else if (lower.Contains("browser"))
            {
                baseName = "webBrowser";
            }
            else if (lower.Contains("panel"))
            {
                baseName = "panel";
            }

            int index = 1;
            string candidate;
            do
            {
                candidate = baseName + index;
                index++;
            }
            while (_nameMap.ContainsKey(candidate));

            return candidate;
        }

        private void AttachDesignHandlers(Control control)
        {
            control.MouseDown += Control_MouseDown;
            control.MouseMove += Control_MouseMove;
            control.MouseUp += Control_MouseUp;
            control.MouseClick += Control_MouseClick;
            control.Paint += Control_Paint;
            control.ContextMenuStrip = _controlContextMenu;
            control.Cursor = Cursors.SizeAll;

            foreach (Control child in control.Controls)
            {
                child.MouseDown += ChildControl_MouseDown;
                child.MouseMove += ChildControl_MouseMove;
                child.MouseUp += ChildControl_MouseUp;
            }
        }

        private void DetachDesignHandlers(Control control)
        {
            control.MouseDown -= Control_MouseDown;
            control.MouseMove -= Control_MouseMove;
            control.MouseUp -= Control_MouseUp;
            control.MouseClick -= Control_MouseClick;
            control.Paint -= Control_Paint;
            control.ContextMenuStrip = null;

            foreach (Control child in control.Controls)
            {
                child.MouseDown -= ChildControl_MouseDown;
                child.MouseMove -= ChildControl_MouseMove;
                child.MouseUp -= ChildControl_MouseUp;
            }
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            Control control = sender as Control;
            if (control == null)
            {
                return;
            }

            SelectControl(control);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            Control control = sender as Control;
            if (control == null)
            {
                return;
            }

            ControlDesignMetadata metadata;
            if (!_controlMap.TryGetValue(control, out metadata))
            {
                return;
            }

            SelectControl(control);

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                _activeDragMetadata = metadata;
                _dragOriginMouseScreen = control.PointToScreen(e.Location);
                _dragOriginControl = control.Location;
                _dragOriginSize = control.Size;
                _isDragging = e.Button == MouseButtons.Left;
                _isResizing = e.Button == MouseButtons.Right;
                control.Capture = true;
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (_activeDragMetadata == null)
            {
                return;
            }

            Control control = _activeDragMetadata.Control;
            Point currentMouse = control.PointToScreen(e.Location);
            int deltaX = currentMouse.X - _dragOriginMouseScreen.X;
            int deltaY = currentMouse.Y - _dragOriginMouseScreen.Y;

            if (_isDragging)
            {
                Point newLocation = new Point(_dragOriginControl.X + deltaX, _dragOriginControl.Y + deltaY);
                if (newLocation.X < 0)
                {
                    newLocation.X = 0;
                }
                if (newLocation.Y < 0)
                {
                    newLocation.Y = 0;
                }

                control.Location = newLocation;
                _propertyGrid.Refresh();
                _formSurface.Invalidate();
            }
            else if (_isResizing)
            {
                int newWidth = Math.Max(16, _dragOriginSize.Width + deltaX);
                int newHeight = Math.Max(16, _dragOriginSize.Height + deltaY);
                control.Size = new Size(newWidth, newHeight);
                _propertyGrid.Refresh();
                _formSurface.Invalidate();
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                control.Capture = false;
            }

            if (_activeDragMetadata != null)
            {
                CommitControlBounds(_activeDragMetadata);
            }

            _activeDragMetadata = null;
            _isDragging = false;
            _isResizing = false;
        }

        private void ChildControl_MouseDown(object sender, MouseEventArgs e)
        {
            Control child = sender as Control;
            if (child == null)
            {
                return;
            }

            Control parent = child.Parent;
            if (parent != null)
            {
                Control_MouseDown(parent, e);
            }
        }

        private void ChildControl_MouseMove(object sender, MouseEventArgs e)
        {
            Control child = sender as Control;
            if (child == null)
            {
                return;
            }

            Control parent = child.Parent;
            if (parent != null)
            {
                Control_MouseMove(parent, e);
            }
        }

        private void ChildControl_MouseUp(object sender, MouseEventArgs e)
        {
            Control child = sender as Control;
            if (child == null)
            {
                return;
            }

            Control parent = child.Parent;
            if (parent != null)
            {
                Control_MouseUp(parent, e);
            }
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            Control control = sender as Control;
            if (control == null || control != _selectedControl)
            {
                return;
            }

            Rectangle rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
            using (Pen pen = new Pen(Color.DeepSkyBlue, 2))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void CommitControlBounds(ControlDesignMetadata metadata)
        {
            Control control = metadata.Control;
            metadata.Definition.Location = new[] { control.Left, control.Top };
            metadata.Definition.Size = new[] { control.Width, control.Height };
            MarkDirty();
            _propertyGrid.Refresh();
            _formSurface.Invalidate();
        }

        private void FormSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SelectControl(null);
            }
        }

        private void FormSurface_Paint(object sender, PaintEventArgs e)
        {
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _formSurface.Invalidate();
        }

        private void ControlContextMenu_Opening(object sender, CancelEventArgs e)
        {
            Control source = _controlContextMenu.SourceControl;
            if (source == null)
            {
                e.Cancel = true;
                return;
            }

            ControlDesignMetadata metadata;
            if (!_controlMap.TryGetValue(source, out metadata))
            {
                e.Cancel = true;
                return;
            }

            SelectControl(source);

            bool isPicture = string.Equals(metadata.Type, "PictureBox", StringComparison.OrdinalIgnoreCase);
            _setImageMenuItem.Visible = isPicture;
            _clearImageMenuItem.Visible = isPicture;
            _setBackgroundMenuItem.Enabled = true;
            _clearBackgroundMenuItem.Enabled = true;
        }

        private void FormContextMenu_Opening(object sender, CancelEventArgs e)
        {
            SelectControl(null);
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedControl();
        }

        private void BringToFrontMenuItem_Click(object sender, EventArgs e)
        {
            BringSelectedControlToFront();
        }

        private void SendToBackMenuItem_Click(object sender, EventArgs e)
        {
            SendSelectedControlToBack();
        }

        private void SetBackgroundMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            string path = BrowseForImagePath();
            if (!string.IsNullOrEmpty(path))
            {
                SetControlBackgroundImage(_selectedMetadata, path);
            }
        }

        private void ClearBackgroundMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            SetControlBackgroundImage(_selectedMetadata, string.Empty);
        }

        private void SetImageMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            string path = BrowseForImagePath();
            if (!string.IsNullOrEmpty(path))
            {
                SetControlImage(_selectedMetadata, path);
            }
        }

        private void ClearImageMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            SetControlImage(_selectedMetadata, string.Empty);
        }

        private void SetFormBackgroundMenuItem_Click(object sender, EventArgs e)
        {
            string path = BrowseForImagePath();
            if (!string.IsNullOrEmpty(path))
            {
                SetFormBackgroundImage(path);
            }
        }

        private void ClearFormBackgroundMenuItem_Click(object sender, EventArgs e)
        {
            SetFormBackgroundImage(string.Empty);
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptSaveChanges())
            {
                return;
            }

            NewLayout();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptSaveChanges())
            {
                return;
            }

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Arquivos de layout (*.json)|*.json|Todos os arquivos (*.*)|*.*";
                dialog.Title = "Abrir layout";
                if (!string.IsNullOrEmpty(_currentFilePath))
                {
                    string directory = Path.GetDirectoryName(_currentFilePath);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        dialog.InitialDirectory = directory;
                    }
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadLayout(dialog.FileName);
                }
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveLayout();
        }

        private void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveLayoutAs();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!PromptSaveChanges())
            {
                e.Cancel = true;
            }
        }

        private void EditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                if (SaveLayout())
                {
                    e.Handled = true;
                }
                return;
            }

            if (e.Control && e.KeyCode == Keys.O)
            {
                OpenMenuItem_Click(this, EventArgs.Empty);
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.N)
            {
                NewMenuItem_Click(this, EventArgs.Empty);
                e.Handled = true;
                return;
            }

            if (_selectedMetadata != null)
            {
                int step = e.Control ? 10 : 1;
                Point location = _selectedMetadata.Control.Location;
                bool moved = false;

                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        DeleteSelectedControl();
                        e.Handled = true;
                        return;
                    case Keys.Left:
                        location.X = Math.Max(0, location.X - step);
                        moved = true;
                        break;
                    case Keys.Right:
                        location.X = Math.Max(0, location.X + step);
                        moved = true;
                        break;
                    case Keys.Up:
                        location.Y = Math.Max(0, location.Y - step);
                        moved = true;
                        break;
                    case Keys.Down:
                        location.Y = Math.Max(0, location.Y + step);
                        moved = true;
                        break;
                }

                if (moved)
                {
                    _selectedMetadata.Control.Location = location;
                    CommitControlBounds(_selectedMetadata);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
            }
        }

        private void SelectControl(Control control)
        {
            if (_selectedControl == control)
            {
                return;
            }

            Control previous = _selectedControl;
            _selectedControl = control;
            _selectedMetadata = null;

            if (previous != null)
            {
                previous.Invalidate();
            }

            if (control != null)
            {
                ControlDesignMetadata metadata;
                if (_controlMap.TryGetValue(control, out metadata))
                {
                    _selectedMetadata = metadata;
                }
                control.Invalidate();
            }

            UpdatePropertyGridSelection();
        }

        private void UpdatePropertyGridSelection()
        {
            if (_selectedMetadata != null)
            {
                if (string.Equals(_selectedMetadata.Type, "PictureBox", StringComparison.OrdinalIgnoreCase))
                {
                    _propertyGrid.SelectedObject = new PictureControlPropertyView(this, _selectedMetadata);
                }
                else
                {
                    _propertyGrid.SelectedObject = new ControlPropertyView(this, _selectedMetadata);
                }
            }
            else
            {
                _propertyGrid.SelectedObject = _formPropertyView;
            }
        }

        private bool PromptSaveChanges()
        {
            if (!_isDirty)
            {
                return true;
            }

            DialogResult result = MessageBox.Show(this, "Deseja salvar as alterações antes de continuar?", "Salvar alterações", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                return false;
            }

            if (result == DialogResult.Yes)
            {
                return SaveLayout();
            }

            return true;
        }

        private void LoadLayout(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                LayoutDefinition definition = serializer.Deserialize<LayoutDefinition>(json);
                if (definition == null)
                {
                    throw new InvalidOperationException("O arquivo não contém um layout válido.");
                }

                _currentLayout = definition;
                _formDefinition = definition.Form ?? new FormDefinition();
                _formBackgroundImagePath = _formDefinition.BackgroundImage;
                _currentFilePath = filePath;
                _nextControlLocation = new Point(24, 24);

                ClearDesignSurface();
                ApplyFormDefinition();

                if (definition.Controls != null)
                {
                    foreach (KeyValuePair<string, ControlDefinition> entry in definition.Controls)
                    {
                        CreateControlFromDefinition(entry.Key, entry.Value);
                    }
                }

                SelectControl(null);
                _isDirty = false;
                UpdateWindowTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Não foi possível carregar o layout: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateControlFromDefinition(string name, ControlDefinition definition)
        {
            string typeName = ResolveControlType(definition.Type, name);
            Control control = CreateDesignControl(typeName);
            if (control == null)
            {
                return;
            }

            control.Name = name;

            if (definition.Location != null && definition.Location.Length >= 2)
            {
                control.Location = new Point(definition.Location[0], definition.Location[1]);
            }

            if (definition.Size != null && definition.Size.Length >= 2)
            {
                control.Size = new Size(definition.Size[0], definition.Size[1]);
            }

            if (!string.IsNullOrEmpty(definition.Text))
            {
                control.Text = definition.Text;
            }

            if (definition.Visible.HasValue)
            {
                control.Visible = definition.Visible.Value;
            }

            if (definition.Enabled.HasValue)
            {
                control.Enabled = definition.Enabled.Value;
            }

            ControlDesignMetadata metadata = new ControlDesignMetadata(control, name, typeName);
            metadata.Definition.Location = new[] { control.Left, control.Top };
            metadata.Definition.Size = new[] { control.Width, control.Height };
            metadata.Definition.Text = definition.Text;
            metadata.Definition.Visible = definition.Visible;
            metadata.Definition.Enabled = definition.Enabled;
            metadata.Definition.BackgroundImage = definition.BackgroundImage;
            metadata.Definition.Image = definition.Image;
            metadata.Definition.SizeMode = definition.SizeMode;

            metadata.BackgroundImagePath = definition.BackgroundImage;
            metadata.ImagePath = definition.Image;

            _formSurface.Controls.Add(control);
            AttachDesignHandlers(control);

            _controlMap[control] = metadata;
            _nameMap[name] = metadata;

            if (!string.IsNullOrEmpty(metadata.BackgroundImagePath))
            {
                ApplyBackgroundImage(metadata, metadata.BackgroundImagePath, false);
            }

            if (!string.IsNullOrEmpty(metadata.ImagePath))
            {
                ApplyImage(metadata, metadata.ImagePath, false);
            }

            if (!string.IsNullOrEmpty(definition.SizeMode))
            {
                ApplySizeMode(metadata, definition.SizeMode);
            }
        }

        private string ResolveControlType(string typeName, string controlName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            if (!string.IsNullOrEmpty(controlName))
            {
                string lower = controlName.ToLowerInvariant();
                if (lower.Contains("button"))
                {
                    return "Button";
                }
                if (lower.Contains("picture"))
                {
                    return "PictureBox";
                }
                if (lower.Contains("browser"))
                {
                    return "WebBrowser";
                }
                if (lower.Contains("panel"))
                {
                    return "Panel";
                }
                if (lower.Contains("text"))
                {
                    return "Label";
                }
            }

            return "Label";
        }

        private bool SaveLayout()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                return SaveLayoutAs();
            }

            try
            {
                SaveLayoutToFile(_currentFilePath);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Não foi possível salvar o layout: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SaveLayoutAs()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Arquivos de layout (*.json)|*.json|Todos os arquivos (*.*)|*.*";
                dialog.Title = "Salvar layout";
                if (!string.IsNullOrEmpty(_currentFilePath))
                {
                    string directory = Path.GetDirectoryName(_currentFilePath);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        dialog.InitialDirectory = directory;
                    }
                    dialog.FileName = Path.GetFileName(_currentFilePath);
                }
                else
                {
                    dialog.FileName = "layout.json";
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        SaveLayoutToFile(dialog.FileName);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Não foi possível salvar o layout: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return false;
        }

        private void SaveLayoutToFile(string filePath)
        {
            LayoutDefinition definition = BuildLayoutDefinition();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(definition);
            string formatted = FormatJson(json);
            File.WriteAllText(filePath, formatted, Encoding.UTF8);

            _currentLayout = definition;
            _currentFilePath = filePath;
            _isDirty = false;
            UpdateWindowTitle();
        }

        private LayoutDefinition BuildLayoutDefinition()
        {
            LayoutDefinition definition = new LayoutDefinition();
            FormDefinition form = new FormDefinition();
            form.ClientSize = new[] { _formSurface.Width, _formSurface.Height };
            form.Text = _formDefinition.Text;
            form.Visible = _formDefinition.Visible;
            form.BackgroundImage = string.IsNullOrEmpty(_formBackgroundImagePath) ? null : _formBackgroundImagePath;
            definition.Form = form;

            Dictionary<string, ControlDefinition> controls = new Dictionary<string, ControlDefinition>();
            foreach (KeyValuePair<string, ControlDesignMetadata> entry in _nameMap)
            {
                ControlDesignMetadata metadata = entry.Value;
                ControlDefinition controlDefinition = new ControlDefinition();
                controlDefinition.Type = metadata.Type;
                controlDefinition.Location = new[] { metadata.Control.Left, metadata.Control.Top };
                controlDefinition.Size = new[] { metadata.Control.Width, metadata.Control.Height };
                controlDefinition.Text = metadata.Definition.Text;
                controlDefinition.Visible = metadata.Definition.Visible;
                controlDefinition.Enabled = metadata.Definition.Enabled;
                controlDefinition.BackgroundImage = string.IsNullOrEmpty(metadata.BackgroundImagePath) ? null : metadata.BackgroundImagePath;
                controlDefinition.Image = string.IsNullOrEmpty(metadata.ImagePath) ? null : metadata.ImagePath;
                controlDefinition.SizeMode = metadata.Definition.SizeMode;
                controls[metadata.Name] = controlDefinition;
            }

            definition.Controls = controls;
            return definition;
        }

        private string FormatJson(string json)
        {
            StringBuilder builder = new StringBuilder();
            bool quoted = false;
            bool escaped = false;
            int indent = 0;

            foreach (char ch in json)
            {
                if (escaped)
                {
                    escaped = false;
                    builder.Append(ch);
                    continue;
                }

                if (ch == '\\')
                {
                    escaped = true;
                    builder.Append(ch);
                    continue;
                }

                if (ch == '"')
                {
                    quoted = !quoted;
                    builder.Append(ch);
                    continue;
                }

                if (!quoted)
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            builder.Append(ch);
                            builder.AppendLine();
                            indent++;
                            builder.Append(new string(' ', indent * 2));
                            break;
                        case '}':
                        case ']':
                            builder.AppendLine();
                            indent = Math.Max(0, indent - 1);
                            builder.Append(new string(' ', indent * 2));
                            builder.Append(ch);
                            break;
                        case ',':
                            builder.Append(ch);
                            builder.AppendLine();
                            builder.Append(new string(' ', indent * 2));
                            break;
                        case ':':
                            builder.Append(ch);
                            builder.Append(' ');
                            break;
                        default:
                            if (!char.IsWhiteSpace(ch))
                            {
                                builder.Append(ch);
                            }
                            break;
                    }
                }
                else
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }

        private void DeleteSelectedControl()
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            Control control = _selectedMetadata.Control;
            DetachDesignHandlers(control);
            _controlMap.Remove(control);
            _nameMap.Remove(_selectedMetadata.Name);
            control.Dispose();
            _selectedMetadata = null;
            _selectedControl = null;
            UpdatePropertyGridSelection();
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void BringSelectedControlToFront()
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            _selectedMetadata.Control.BringToFront();
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void SendSelectedControlToBack()
        {
            if (_selectedMetadata == null)
            {
                return;
            }

            _selectedMetadata.Control.SendToBack();
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void SetControlBackgroundImage(ControlDesignMetadata metadata, string path)
        {
            ApplyBackgroundImage(metadata, path, true);
            UpdatePropertyGridSelection();
        }

        private void ApplyBackgroundImage(ControlDesignMetadata metadata, string path, bool markDirty)
        {
            Control control = metadata.Control;
            Image previous = control.BackgroundImage;

            if (string.IsNullOrEmpty(path))
            {
                control.BackgroundImage = null;
                metadata.BackgroundImagePath = null;
                metadata.Definition.BackgroundImage = null;
            }
            else
            {
                Image image = LoadImageAsset(path);
                if (image != null)
                {
                    control.BackgroundImage = image;
                    control.BackgroundImageLayout = ImageLayout.Stretch;
                    metadata.BackgroundImagePath = path;
                    metadata.Definition.BackgroundImage = path;
                }
                else
                {
                    metadata.BackgroundImagePath = path;
                    metadata.Definition.BackgroundImage = path;
                    control.BackgroundImage = null;
                    if (markDirty)
                    {
                        MessageBox.Show(this, "Não foi possível carregar a imagem informada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            if (previous != null)
            {
                previous.Dispose();
            }

            if (markDirty)
            {
                MarkDirty();
            }
            _formSurface.Invalidate();
        }

        private void SetControlImage(ControlDesignMetadata metadata, string path)
        {
            ApplyImage(metadata, path, true);
            UpdatePropertyGridSelection();
        }

        private void ApplyImage(ControlDesignMetadata metadata, string path, bool markDirty)
        {
            PictureBox pictureBox = metadata.Control as PictureBox;
            if (pictureBox == null)
            {
                metadata.ImagePath = null;
                metadata.Definition.Image = null;
                return;
            }

            Image previous = pictureBox.Image;

            if (string.IsNullOrEmpty(path))
            {
                pictureBox.Image = null;
                metadata.ImagePath = null;
                metadata.Definition.Image = null;
            }
            else
            {
                Image image = LoadImageAsset(path);
                if (image != null)
                {
                    pictureBox.Image = image;
                    metadata.ImagePath = path;
                    metadata.Definition.Image = path;
                }
                else
                {
                    metadata.ImagePath = path;
                    metadata.Definition.Image = path;
                    pictureBox.Image = null;
                    if (markDirty)
                    {
                        MessageBox.Show(this, "Não foi possível carregar a imagem informada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            if (previous != null)
            {
                previous.Dispose();
            }

            if (markDirty)
            {
                MarkDirty();
            }
            _formSurface.Invalidate();
        }

        private void ApplySizeMode(ControlDesignMetadata metadata, string sizeMode)
        {
            PictureBox pictureBox = metadata.Control as PictureBox;
            if (pictureBox == null)
            {
                return;
            }

            try
            {
                PictureBoxSizeMode mode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), sizeMode, true);
                pictureBox.SizeMode = mode;
                metadata.Definition.SizeMode = mode.ToString();
            }
            catch
            {
            }
        }

        private void SetPictureSizeMode(ControlDesignMetadata metadata, PictureBoxSizeMode mode)
        {
            PictureBox pictureBox = metadata.Control as PictureBox;
            if (pictureBox == null)
            {
                return;
            }

            pictureBox.SizeMode = mode;
            metadata.Definition.SizeMode = mode.ToString();
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void RenameControl(ControlDesignMetadata metadata, string newName)
        {
            if (string.IsNullOrEmpty(newName) || string.Equals(metadata.Name, newName, StringComparison.Ordinal))
            {
                return;
            }

            if (_nameMap.ContainsKey(newName))
            {
                MessageBox.Show(this, "Já existe um controle com esse nome.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _nameMap.Remove(metadata.Name);
            metadata.Name = newName;
            metadata.Control.Name = newName;
            _nameMap[newName] = metadata;
            MarkDirty();
        }

        private void ChangeControlType(ControlDesignMetadata metadata, string newType)
        {
            if (string.IsNullOrEmpty(newType) || string.Equals(metadata.Type, newType, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Control newControl = CreateDesignControl(newType);
            if (newControl == null)
            {
                MessageBox.Show(this, "Não foi possível criar o controle do tipo especificado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Control oldControl = metadata.Control;
            newControl.Name = metadata.Name;
            newControl.Location = oldControl.Location;
            newControl.Size = oldControl.Size;
            newControl.Text = oldControl.Text;

            DetachDesignHandlers(oldControl);
            int index = _formSurface.Controls.GetChildIndex(oldControl);
            _formSurface.Controls.Add(newControl);
            _formSurface.Controls.SetChildIndex(newControl, index);
            _formSurface.Controls.Remove(oldControl);
            oldControl.Dispose();

            metadata.Control = newControl;
            metadata.Type = newType;
            metadata.Definition.Type = newType;
            metadata.Definition.Text = newControl.Text;

            if (!string.Equals(newType, "PictureBox", StringComparison.OrdinalIgnoreCase))
            {
                metadata.ImagePath = null;
                metadata.Definition.Image = null;
                metadata.Definition.SizeMode = null;
            }

            _controlMap.Remove(oldControl);
            _controlMap[newControl] = metadata;

            AttachDesignHandlers(newControl);

            if (!string.IsNullOrEmpty(metadata.BackgroundImagePath))
            {
                ApplyBackgroundImage(metadata, metadata.BackgroundImagePath, false);
            }

            if (!string.IsNullOrEmpty(metadata.ImagePath))
            {
                ApplyImage(metadata, metadata.ImagePath, false);
            }

            SelectControl(newControl);
            MarkDirty();
        }

        private void SetControlText(ControlDesignMetadata metadata, string text)
        {
            metadata.Control.Text = text;
            metadata.Definition.Text = text;
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void SetControlLocation(ControlDesignMetadata metadata, Point location)
        {
            if (location.X < 0)
            {
                location.X = 0;
            }
            if (location.Y < 0)
            {
                location.Y = 0;
            }

            metadata.Control.Location = location;
            metadata.Definition.Location = new[] { location.X, location.Y };
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void SetControlSize(ControlDesignMetadata metadata, Size size)
        {
            if (size.Width < 16)
            {
                size.Width = 16;
            }
            if (size.Height < 16)
            {
                size.Height = 16;
            }

            metadata.Control.Size = size;
            metadata.Definition.Size = new[] { size.Width, size.Height };
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void SetControlVisible(ControlDesignMetadata metadata, bool visible)
        {
            metadata.Control.Visible = visible;
            metadata.Definition.Visible = visible;
            MarkDirty();
        }

        private void SetControlEnabled(ControlDesignMetadata metadata, bool enabled)
        {
            metadata.Control.Enabled = enabled;
            metadata.Definition.Enabled = enabled;
            MarkDirty();
        }

        private void SetFormText(string text)
        {
            _formDefinition.Text = string.IsNullOrEmpty(text) ? null : text;
            SetFormTextInternal(_formDefinition.Text);
            MarkDirty();
        }

        private void SetFormTextInternal(string text)
        {
            string caption = string.IsNullOrEmpty(text) ? "(sem título)" : text;
            _formTitleLabel.Text = caption;
        }

        private void SetFormSize(Size size)
        {
            if (size.Width < 200)
            {
                size.Width = 200;
            }
            if (size.Height < 200)
            {
                size.Height = 200;
            }

            _formSurface.Size = size;
            _formDefinition.ClientSize = new[] { size.Width, size.Height };
            MarkDirty();
            _formSurface.Invalidate();
        }

        private void SetFormVisible(bool visible)
        {
            _formDefinition.Visible = visible;
            MarkDirty();
        }

        private void SetFormBackgroundImage(string path)
        {
            ApplyFormBackgroundImage(path, true);
            UpdatePropertyGridSelection();
        }

        private void ApplyFormBackgroundImage(string path, bool markDirty)
        {
            Image previous = _formSurface.BackgroundImage;

            if (string.IsNullOrEmpty(path))
            {
                _formSurface.BackgroundImage = null;
                _formBackgroundImagePath = null;
                _formDefinition.BackgroundImage = null;
            }
            else
            {
                Image image = LoadImageAsset(path);
                if (image != null)
                {
                    _formSurface.BackgroundImage = image;
                    _formSurface.BackgroundImageLayout = ImageLayout.Stretch;
                    _formBackgroundImagePath = path;
                    _formDefinition.BackgroundImage = path;
                }
                else
                {
                    _formSurface.BackgroundImage = null;
                    _formBackgroundImagePath = path;
                    _formDefinition.BackgroundImage = path;
                    if (markDirty)
                    {
                        MessageBox.Show(this, "Não foi possível carregar a imagem informada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            if (previous != null)
            {
                previous.Dispose();
            }

            if (markDirty)
            {
                MarkDirty();
            }

            _formSurface.Invalidate();
        }

        private Image LoadImageAsset(string assetPath)
        {
            try
            {
                string absolutePath = GetAbsoluteAssetPath(assetPath);
                if (string.IsNullOrEmpty(absolutePath) || !File.Exists(absolutePath))
                {
                    return null;
                }

                using (FileStream stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        CopyStream(stream, memory);
                        memory.Position = 0;
                        using (Image image = Image.FromStream(memory))
                        {
                            return new Bitmap(image);
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            byte[] buffer = new byte[4096];
            int count;
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, count);
            }
        }

        private string GetAbsoluteAssetPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            if (Path.IsPathRooted(assetPath))
            {
                return Path.GetFullPath(assetPath);
            }

            string baseDirectory = !string.IsNullOrEmpty(_currentFilePath) ? Path.GetDirectoryName(_currentFilePath) : Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(baseDirectory))
            {
                return Path.GetFullPath(assetPath);
            }

            return Path.GetFullPath(Path.Combine(baseDirectory, assetPath));
        }

        private string BrowseForImagePath()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Arquivos de imagem|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Todos os arquivos (*.*)|*.*";
                dialog.Title = "Selecionar imagem";
                if (!string.IsNullOrEmpty(_currentFilePath))
                {
                    string directory = Path.GetDirectoryName(_currentFilePath);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        dialog.InitialDirectory = directory;
                    }
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    return NormalizeAssetPath(dialog.FileName);
                }
            }

            return null;
        }

        private string NormalizeAssetPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(_currentFilePath))
            {
                return fullPath;
            }

            try
            {
                string baseDirectory = Path.GetDirectoryName(_currentFilePath);
                if (string.IsNullOrEmpty(baseDirectory))
                {
                    return fullPath;
                }

                string fullBase = Path.GetFullPath(baseDirectory) + Path.DirectorySeparatorChar;
                string target = Path.GetFullPath(fullPath);
                if (target.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase))
                {
                    return target.Substring(fullBase.Length);
                }
            }
            catch
            {
            }

            return fullPath;
        }

        private void UpdateWindowTitle()
        {
            string fileName = string.IsNullOrEmpty(_currentFilePath) ? "sem título" : Path.GetFileName(_currentFilePath);
            Text = "Editor de Layout - " + fileName + (_isDirty ? "*" : string.Empty);
        }

        private void MarkDirty()
        {
            if (!_isDirty)
            {
                _isDirty = true;
                UpdateWindowTitle();
            }
        }

        private class ControlDesignMetadata
        {
            public ControlDesignMetadata(Control control, string name, string type)
            {
                Control = control;
                Name = name;
                Type = type;
                Definition = new ControlDefinition();
                Definition.Type = type;
            }

            public Control Control { get; set; }
            public ControlDefinition Definition { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string BackgroundImagePath { get; set; }
            public string ImagePath { get; set; }
        }

        private class ControlPropertyView
        {
            protected readonly EditorForm Owner;
            protected readonly ControlDesignMetadata Metadata;

            public ControlPropertyView(EditorForm owner, ControlDesignMetadata metadata)
            {
                Owner = owner;
                Metadata = metadata;
            }

            [Category("Identidade"), DisplayName("Nome")]
            public string Name
            {
                get { return Metadata.Name; }
                set { Owner.RenameControl(Metadata, value); }
            }

            [Category("Identidade"), DisplayName("Tipo"), TypeConverter(typeof(ControlTypeConverter))]
            public string Type
            {
                get { return Metadata.Type; }
                set { Owner.ChangeControlType(Metadata, value); }
            }

            [Category("Layout"), DisplayName("Posição")]
            public Point Location
            {
                get { return Metadata.Control.Location; }
                set { Owner.SetControlLocation(Metadata, value); }
            }

            [Category("Layout"), DisplayName("Tamanho")]
            public Size Size
            {
                get { return Metadata.Control.Size; }
                set { Owner.SetControlSize(Metadata, value); }
            }

            [Category("Aparência"), DisplayName("Texto")]
            public string Text
            {
                get { return Metadata.Control.Text; }
                set { Owner.SetControlText(Metadata, value); }
            }

            [Category("Aparência"), DisplayName("Imagem de fundo")]
            public string BackgroundImage
            {
                get { return Metadata.BackgroundImagePath ?? string.Empty; }
                set { Owner.SetControlBackgroundImage(Metadata, value); }
            }

            [Category("Comportamento"), DisplayName("Visível")]
            public bool Visible
            {
                get { return Metadata.Control.Visible; }
                set { Owner.SetControlVisible(Metadata, value); }
            }

            [Category("Comportamento"), DisplayName("Habilitado")]
            public bool Enabled
            {
                get { return Metadata.Control.Enabled; }
                set { Owner.SetControlEnabled(Metadata, value); }
            }
        }

        private class PictureControlPropertyView : ControlPropertyView
        {
            public PictureControlPropertyView(EditorForm owner, ControlDesignMetadata metadata) : base(owner, metadata)
            {
            }

            [Category("Aparência"), DisplayName("Imagem")]
            public string Image
            {
                get { return Metadata.ImagePath ?? string.Empty; }
                set { Owner.SetControlImage(Metadata, value); }
            }

            [Category("Aparência"), DisplayName("Modo de escala")]
            public PictureBoxSizeMode SizeMode
            {
                get
                {
                    PictureBox pictureBox = Metadata.Control as PictureBox;
                    return pictureBox != null ? pictureBox.SizeMode : PictureBoxSizeMode.Zoom;
                }
                set
                {
                    Owner.SetPictureSizeMode(Metadata, value);
                }
            }
        }

        private class FormPropertyView
        {
            private readonly EditorForm _owner;

            public FormPropertyView(EditorForm owner)
            {
                _owner = owner;
            }

            [Category("Aparência"), DisplayName("Título")]
            public string Text
            {
                get { return _owner._formDefinition.Text ?? string.Empty; }
                set { _owner.SetFormText(value); }
            }

            [Category("Layout"), DisplayName("Tamanho")]
            public Size ClientSize
            {
                get { return _owner._formSurface.Size; }
                set { _owner.SetFormSize(value); }
            }

            [Category("Aparência"), DisplayName("Imagem de fundo")]
            public string BackgroundImage
            {
                get { return _owner._formBackgroundImagePath ?? string.Empty; }
                set { _owner.SetFormBackgroundImage(value); }
            }

            [Category("Comportamento"), DisplayName("Visível")]
            public bool Visible
            {
                get { return _owner._formDefinition.Visible ?? true; }
                set { _owner.SetFormVisible(value); }
            }
        }

        private class ControlTypeConverter : StringConverter
        {
            private static readonly string[] Types = { "Label", "Button", "PictureBox", "TextBox", "WebBrowser", "Panel" };

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(Types);
            }
        }

        private class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                DoubleBuffered = true;
            }
        }
    }
}
