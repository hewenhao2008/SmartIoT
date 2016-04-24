<?php
session_start();
if(isset($_SESSION['logged_in']) && !empty($_SESSION['logged_in'])){
header("Location: index.php");
}

?>
<html>
<head>
<title>Log in or sign up</title>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"/>
<script src="jquery.js"></script>
<script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
<style>
html,body{
margin:0;
padding:0;
width:100%;
height:100%;
overflow:auto;
}
body{
background:white;
}

.noselect *{
-webkit-touch-callout: none; /* iOS Safari */
-webkit-user-select: none;   /* Chrome/Safari/Opera */
-khtml-user-select: none;    /* Konqueror */
-moz-user-select: none;      /* Firefox */
-ms-user-select: none;       /* IE/Edge */
user-select: none;           /* non-prefixed version, currently
                                not supported by any browser */
}
.logo{
position:relative;
width:100%;
max-width:100%;
margin:0 auto;
text-align:center;

}
.logo>img{
max-width:100%;
display:inline-block;
}
h4{
display:inline-block;
}
.platform{
position:relative;
background:rgba(174, 193, 193, 0.36);
width:500px;
max-width:100%;
margin:10px auto;
padding:20px;
overflow:auto;
}
.user_img{
display:block;
max-width:100px;
margin:auto;
margin-top:20px;
margin-bottom:10px;
}
.image_circle{
margin:auto;
margin-top:20px;
margin-bottom:30px;
max-width:150px;
background:wheat;
padding:10px;
border-radius:50%;
}
.naslov{
position:relative;
width:100%;
text-align:center;
}
.nav{
margin-bottom:30px;
}
.error{
color:red;
display:none;
font-size:1.1em;
}
.success{
color:green;
display:none;
font-size:1.1.em;s
}
</style>
</head>
<body>
<div class="logo noselect">
<img  src="img/elisium.png" alt="Your house is our care"/>
<h4>Your devices are our concern</h4>
</div>
<div class="platform">

<div class="image_circle">
<img class="user_img" src="img/user.png"/>
</div>

  <h2 style="color:rgba(7, 123, 247, 0.56);margin-bottom:20px" class="naslov">Sign in</h2>

  <div id="login" class="tab-pane fade in active">
    <div class="login_form text-center">
    <table style="display:inline-block;margin-bottom:20px;">
    <tr>
    <td>
    <span>User ID :  </span>
    </td>
    <td>
    <input id="uid" type="text" placeholder="UID"></input>
    </td>
    </tr>
    <tr><td><br></td></tr>
    <tr>
    <td> <span>Password :  </span></td><td><input type="password" id="passwd" type="text" placeholder="Password"></input></td></tr>
    </table>
    <br>
    <button class="btn-lg btn-info" id="login_btn">Uloguj se</button>
    <br>
    <span class="error"></span>
    <span class="success"></span>
    </div>
    </div>

</div>
<script type="text/javascript">
$('#login_btn').click(function(){ 
var uid = $('#uid').val();
var pass = $('#passwd').val();
if(uid != "" && pass != ""){
$.post( "bin/login.php", { uid: uid, pass: pass})
  .done(function( data ) {
    if(data=="OK"){
        $('#login_btn').prop('disabled','true').removeClass('btn-info').addClass('btn-success');
         $('.success').html("Uspjesno ste logovani").css('display','block').fadeOut(2000);
        setTimeout(function(){window.location="index.php"},2000);
        
    }else{
        $('.error').html(data).css('display','block').fadeOut(3000);
    }
  })
  .fail(function(data){
    $('.error').html("Server nije dostupan").css('display','block').fadeOut(3000);
  });
}else{
  $('.error').html("Morate unijeti sve podatke").css('display','block').fadeOut(3000);
}

});
</script>
</body>
</html>
