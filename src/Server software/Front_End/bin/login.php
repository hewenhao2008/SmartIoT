<?php
$SITE_ROOT = "../";
include_once($SITE_ROOT.'conf.php');
if(isset($_POST['uid']) && isset($_POST['pass']) && !empty($_POST['uid']) && !empty($_POST['pass'])){
$conn = new mysqli($server, $user, $pass, $db);
if ($conn->connect_error) {
    die("Server ERROR");
}

$stmt = $conn->prepare("SELECT COUNT(*) AS total FROM user_login WHERE uid = ? AND pass = ?");
$stmt->bind_param('is', $uid, $pass);
// set parameters and execute
$uid = $_POST['uid'];
$pass = $_POST['pass'];
$stmt->execute();
$data =$stmt->get_result()->fetch_assoc();
if($data['total']==1){
session_start();
session_regenerate_id();
$_SESSION['logged_in'] = $uid;
$_SESSION['pass'] = $pass;
echo "OK";
}else{
echo "Provjerite UID i sifru!";
}
}else{
die("Access denied");
}


?>
