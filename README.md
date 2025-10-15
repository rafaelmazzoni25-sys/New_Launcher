# Launcher – Personalização de Layout

Este repositório contém o **Launcher** original e as ferramentas necessárias para personalizar o seu layout de forma visual. A aplicação passa a carregar, na inicialização, as definições de layout presentes no arquivo `layout.json` (ou `layout.svg`) localizado na mesma pasta do executável.

## Visão geral da personalização
- As definições de layout são descritas pela estrutura `LayoutDefinition` compartilhada no projeto `LayoutContracts`.
- O Launcher procura automaticamente por `layout.json` e, caso não exista, por `layout.svg` no diretório base da aplicação e aplica o conteúdo encontrado.
- Imagens referenciadas em um layout podem ser caminhos absolutos ou caminhos relativos ao arquivo de layout.

## Ferramenta visual: LayoutEditor
Para editar layouts de forma visual foi criado um editor stand-alone localizado em `LayoutEditor/`.

### Como compilar
1. Abra o arquivo `LayoutEditor/LayoutEditor.sln` no Visual Studio 2019 ou mais recente.
2. Restaure os pacotes NuGet se solicitado (nenhum pacote externo é necessário além do .NET Framework 4.x incluído).
3. Compile a solução (`Ctrl+Shift+B`). O executável `LayoutEditor.exe` será gerado em `LayoutEditor/bin/Debug/` (ou `bin/Release/` conforme a configuração escolhida).

### Como usar o LayoutEditor
1. Execute `LayoutEditor.exe`.
2. Utilize o menu **Arquivo ▸ Novo** para iniciar um layout em branco ou **Arquivo ▸ Abrir** para carregar um `layout.json` existente.
3. A lista **Componentes** (lado esquerdo) exibe os tipos de controle suportados: `Label`, `Button`, `PictureBox`, `TextBox`, `WebBrowser` e `Panel`.
   - Dê um duplo clique ou pressione **Enter** em um item da lista para adicioná-lo ao formulário.
4. Posicione controles arrastando-os com o botão esquerdo do mouse. Redimensione arrastando com o botão direito.
5. Use o **PropertyGrid** (painel inferior direito) para editar propriedades como `Nome`, `Tipo`, `Posição`, `Tamanho`, `Texto`, visibilidade e imagens.
   - Atalhos de teclado: **Ctrl+S** (Salvar), **Ctrl+O** (Abrir), **Ctrl+N** (Novo), **Delete** (excluir controle), setas (**←↑→↓**) movem o controle selecionado (use **Ctrl** + seta para mover em passos de 10px).
6. Clique com o botão direito em um controle para acessar opções rápidas: trazer para frente/trás, definir imagem de fundo, definir imagem principal ou excluir.
7. Clique fora de qualquer controle (área cinza) e use o botão direito para definir ou limpar a imagem de fundo do formulário. As propriedades gerais do formulário aparecem na guia "Formulário" do PropertyGrid.
8. Ao salvar, o editor gera um `layout.json` formatado com o mesmo esquema que o Launcher consome. Recursos gráficos são salvos utilizando caminhos relativos ao arquivo quando possível (para facilitar a distribuição do pacote).

### Fluxo de trabalho recomendado
1. Crie ou edite o layout no LayoutEditor e salve o arquivo `layout.json` e todos os ativos de imagem na mesma pasta (ou em subpastas).
2. Copie `layout.json` (e as imagens) para a pasta do Launcher.
3. Inicie o Launcher. Ao detectar `layout.json`, ele aplicará o layout customizado automaticamente.

## Estrutura do arquivo `layout.json`
O arquivo JSON segue a estrutura abaixo:

```json
{
  "Form": {
    "ClientSize": [990, 560],
    "Text": "MU Launcher",
    "BackgroundImage": "imagens/fundo.png"
  },
  "Controls": {
    "NomeDoControle": {
      "Type": "PictureBox",
      "Location": [100, 120],
      "Size": [180, 60],
      "Text": "Iniciar",
      "BackgroundImage": "imagens/botao.png",
      "Image": "imagens/icone.png",
      "Visible": true,
      "Enabled": true,
      "SizeMode": "StretchImage"
    }
  }
}
```

- `Form` descreve tamanho, título e imagem de fundo do formulário principal.
- Cada item em `Controls` corresponde ao `Name` de um controle existente ou novo. Informe o tipo desejado em `Type` caso esteja criando um controle que não existe no layout padrão.
- Coordenadas (`Location`) e tamanhos (`Size`) são arrays `[x, y]` e `[largura, altura]` em pixels.
- `SizeMode` é aplicado a controles que suportam a propriedade (ex.: `PictureBox`). Valores comuns: `Normal`, `StretchImage`, `Zoom`.

## Aplicando layouts SVG
Embora o editor exporte apenas JSON, o Launcher também aceita `layout.svg`. Os atributos reconhecidos são:
- `width` e `height` no elemento raiz (`<svg>`) para definir o tamanho do formulário.
- `data-background`, `data-text` e `display` no elemento `<svg>` ou em elementos com `id` correspondente ao nome de um controle.
- `data-image`, `data-enabled` e `data-sizemode` para personalizar imagens e comportamento de controles.

Basta posicionar `layout.svg` na pasta do Launcher (sem `layout.json`) para que seja carregado.

## Dicas
- Utilize nomes de controle claros (ex.: `BotaoJogar`) para facilitar futuras manutenções.
- Guarde os ativos gráficos na mesma pasta do layout; o editor gera caminhos relativos automaticamente, o que facilita mover o conjunto para outra máquina.
- Em caso de erro durante o carregamento, o Launcher ignora o arquivo e mantém o layout original. Verifique se o JSON está válido e se os caminhos de imagem existem.

Com essas instruções você poderá personalizar completamente o visual do Launcher sem recompilar a aplicação.
