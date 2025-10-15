

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <title> MuOnline</title>
    <link type="text/css" rel="stylesheet" href="assets/css/launcher.css" />
    <script src="assets/js/jquery-1.11.1.min.js"></script>
    <script src="assets/js/jquery-migrate-1.2.1.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/bxslider/4.2.12/jquery.bxslider.min.js"></script>
    <script src="assets/js/ui.js"></script>
    <!--[if lt IE 9]>
    <script src="assets/js/html5.js"></script><![endif]-->
    <script>
        $(document).ready(function () {
            $(document).bind("contextmenu", function (e) {
                return false;
            });
        });
        $(document).bind('selectstart', function () { return false; });
        $(document).bind('dragstart', function () { return false; });
    </script>
</head>
<body>
    <section class="wrp_launcher">
        <article class="launcher">
            <h1 class="blind">launcher</h1>
            <div class="main_banner">
                <ul class="visual_slider">
                        <li><a href="http://exilemu.com" target="_blank"><img src="assets/image/role1.jpg" alt="" /></a></li>
                        <li><a href="http://exilemu.com" target="_blank"><img src="assets/image/role2.jpg" alt="" /></a></li>
                        <li><a href="http://exilemu.com" target="_blank"><img src="assets/image/role3.jpg" alt="" /></a></li>
                </ul>
            </div>
            <div class="contents">
                <div class="top_bx">
                    <ul class="menu_list">
                        <li><a href="http://exilemu.com" target="_blank">Home page</a></li>
                        <li><a href="http://exilemu.com" target="_blank">Shop</a></li>
                        <li><a href="http://exilemu.com" target="_blank">Guide</a></li>
                    </ul>
                    <div class="game_grade">
                        <span class="blind">Violence / Youth not available</span>
                    </div>
                </div>
                <dl class="noti_list">
                    <dt><span>Notice</span><a href="http://exilemu.com" target="_blank">More</a></dt>
                            <dd><a href="http://exilemu.com" target="_blank">Notice 1</a></dd>
                </dl>
                <dl class="noti_list">
                    <dt><span>Patch Notes</span><a href="http://exilemu.com" target="_blank">More</a></dt>
                            <dd><a href="http://exilemu.com" target="_blank">Patch notes 1</a></dd>
                </dl>
                        <a class="ad_link" href="http://exilemu.com" target="_blank">
                            <img src="assets/image/2018101685C94FBDD13F4EBA.jpg" alt="" />
                        </a>
            </div>
        </article>
    </section>
</body>
</html>
