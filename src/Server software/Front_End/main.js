if (Array.prototype.equals)
	console.warn("Overriding existing Array.prototype.equals. Possible causes: New API defines the method, there's a framework conflict or you've got double inclusions in your code.");
Array.prototype.equals = function (array) {
	if (!array)
		return false;
	if (this.length != array.length)
		return false;
	for (var i = 0, l = this.length; i < l; i++) {
		if (this[i]instanceof Array && array[i]instanceof Array) {
			// recurse into the nested arrays
			if (!this[i].equals(array[i]))
				return false;
		} else if (this[i] != array[i]) {
			return false;
		}
	}
	return true;
}
Object.defineProperty(Array.prototype, "equals", {
	enumerable : false
});
//functions override

//devices we have
var glob_devices = [];
function pwmButtonClick() {
	var input = $(this).parent().children('input');
	switchDeviceState(uid, passwd, input.attr("devid"), input.val());
}
function colorButtonClick(){
	var input = $(this).parent().children('.devid_holder');
	switchDeviceState(uid,passwd,input.attr('devid'),"#"+input.attr("value").toString());
}
function createAllDevices(dev_array) {
	glob_devices = dev_array
    $('.device_container > .row').html("");
	dev_array.forEach(function (device) {
		var devid = device[0].toString();
		var state = device[1].toString();
		var name = device[3].toString();
		var html_dev_obj = "";
		html_dev_obj += "<div class='device col-xs-12 col-sm-12 col-md-3 col-lg-2 " + devid + "'>";
		html_dev_obj += "<h3 class='title text-center'>" + name + "</h3><br>";
		html_dev_obj += "<div class='info_cont text-center'>";
		html_dev_obj += "<h4 style='text-decoration:underline;color:yellow'>Osnovne informacije</h4>";
		html_dev_obj += "<span class='destate'>Trenutno stanje : " + state + "</span><br>";
		html_dev_obj += "<span class='dev_id_s'>ID uredjaja : " + devid + "</span>";
		html_dev_obj += "</div>";
		html_dev_obj += "<div class='mogucnosti text-center'>";
		html_dev_obj += "<h4 style='text-decoration:underline;color:yellow'>Mogucnosti upravljanja</h4>";
		html_dev_obj += getDeviceOptions(device);
		html_dev_obj += "</div>";
		html_dev_obj += "</div>";
		$('.device_container > .row').append(html_dev_obj);
	    
	});
	jscolor.installByClassName("jscolor");
}
function getDeviceOptions(device) {
	var ret = "";
	var dev_type = parseInt(device[2]);
	var devid = device[0].toString();
	var state = device[1].toString();
	var name = device[3].toString();
	switch (dev_type) {
	case 0:
		ret = "<button onclick='switchDeviceState(uid,passwd," + devid + ",1)' class='btn btn-default btn-flat btn-success'>Upali uredjaj</button>";
		ret += "<button onclick='switchDeviceState(uid,passwd," + devid + ",0)' class='btn btn-default btn-flat btn-danger'>Ugasi uredjaj</button>";
		return ret;
	case 1:
		ret = "<input devid = "+devid+" type='text' class='pwm_input'></input><br><button class='btn btn-default btn-flat pwm_button btn-info'>Primjeni</button>";
		return ret;
	case 2:
		ret +="<input class='devid_holder' devid='"+devid+"' id='devid-"+devid+"' type='hidden' value='"+state+"'/>";
		ret +="<button class=\"btn btn-default btn-flat jscolor {valueElement:'devid-"+devid+"'}\" value='"+state+"'>Izaberi boju</button><br>";
		ret +="<button class='btn btn-default btn-flat color_button btn-info'>Primjeni</button>";
		return ret;
		break;
	case 3:
		ret = "Ovaj uredjaj nema mogucnosti upravljanja";
		return ret;
		break;

	}
}
function switchDeviceState(uid, password, devid, state) {
	//we send function changestate:uid:passwd:devid:state, we should recieve ok
var query = "changestate:"+uid.toString()+":"+password.toString()+":"+ devid.toString()+":"+ state.toString();
	$.post("el_interface.php", {
		uid : uid,
		q : query
	}).done(function (data) {
		var d = data.toString();
		console.log(d)
	}).fail(function () {
		console.log("Unable to connect to server .."); // maybe even alert here
	});
}

function deviceChanged(dev_id, dev) { // has device changed ?
	if ($('.' + dev_id.toString()).length == 0 || glob_devices.length == 0) {
		createAllDevices(dev);
	} else {
		dev.forEach(function (device, index) {
			if (device[0] == dev_id) {
				var devid = device[0].toString();
				var state = device[1].toString();
				var name = device[3].toString();
				var html_dev_obj = "";
				html_dev_obj += "<div class='device col-xs-12 col-sm-12 col-md-3 col-lg-2 " + devid + "'>";
				html_dev_obj += "<h3 class='title text-center'>" + name + "</h3><br>";
				html_dev_obj += "<div class='info_cont text-center'>";
				html_dev_obj += "<h4 style='text-decoration:underline;color:yellow'>Osnovne informacije</h4>";
				html_dev_obj += "<span class='destate'>Trenutno stanje : " + state + "</span><br>";
				html_dev_obj += "<span class='dev_id_s'>ID uredjaja : " + devid + "</span>";
				html_dev_obj += "</div>";
				html_dev_obj += "<div class='mogucnosti text-center'>";
				html_dev_obj += "<h4 style='text-decoration:underline;color:yellow'>Mogucnosti upravljanja</h4>";
				html_dev_obj += getDeviceOptions(device);
				html_dev_obj += "</div>";
				html_dev_obj += "</div>";
				$('.' + dev_id.toString()).replaceWith(html_dev_obj); //replace in html
				glob_devices[index] = dev[index];// replace in array
			
			}
			if(parseInt(device[2]) == 2){
				jscolor.installByClassName("jscolor");
			}
			
		});

	}
}
function getStates() {
	$.post("el_interface.php", {
		uid : "1",
		q : 'getdevicestate:'+uid+':'+passwd
	}).done(function (data) {
		var d = data.toString();
		d = d.replace(/(\r\n|\r|\n)/gm, "");
		if (d.split(":")[0] == "ERROR") {
			if (d.split(":")[1] == "002") {
				$(".online_state > i").removeClass("text-success").addClass("text-danger");
				$(".online_state > b").html("OFFLINE");
			}
			glob_devices = [];
			$(".device_container > .row").html("");
		} else {
			$(".online_state > i").addClass("text-success").removeClass("text-danger");
			$(".online_state > b").html("ONLINE");
			var devices = data.split(":")[1];
			var dev = $.parseJSON(devices);
			if (glob_devices.length == 0) {
				createAllDevices(dev);
			} else {
				for (var i = 0; i < glob_devices.length; i++) {
					if (glob_devices[i][1] != dev[i][1]) {
						deviceChanged(glob_devices[i][0], dev);
					}
				}
			}

		}
	}).fail(function () {
		$('response').html("NIJE MOGUCE USPOSTAVITI VEZU SA SERVEROM");
	});

	setTimeout(getStates, 200);
}
getStates();
$('.device_container > .row ').on('click','.device > .mogucnosti > .pwm_button',pwmButtonClick);
$('.device_container > .row ').on('click','.device > .mogucnosti > .color_button',colorButtonClick);


