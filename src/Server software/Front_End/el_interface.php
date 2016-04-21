
<?php
error_reporting(0);
if(isset($_POST['uid']) && !empty($_POST['uid']) || isset($_POST['q']) && !empty($_POST['q'])){
$fp = fsockopen("127.0.0.1",5000, $errno, $errstr, 1);
if (!$fp) {
  print("ERROR:003:ELISIUM NIJE ONLINE\n");
} else {
    $input = $_POST['q'] . PHP_EOL;
    fwrite($fp, $input);
    echo fread($fp, 1024);
    fclose($fp);
}
}
?>
