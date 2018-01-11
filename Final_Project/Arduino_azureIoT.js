//var clientFromConnectionString = require('azure-iot-device-mqtt').clientFromConnectionString;

//var configs = require('./configs.js');
//var connectingString = 'HostName=TemperatureLab.azure-devices.net;DeviceId=Linkit7688;SharedAccessKey=UuedpHhVxcBb7l5AhElnn8fMH9RPtJBqKVu2W153+Gk=';
//var client = clientFromConnectionString(connectingString);
const Client = require('azure-iot-device').Client;
const ConnectionString = require('azure-iot-device').ConnectionString;
const Message = require('azure-iot-device').Message;
const Protocol = require('azure-iot-device-amqp').Amqp;

//var DeviceID = "IT_Booth_Device";
var ConString = "HostName=IoThub0104.azure-devices.net;DeviceId=Devices;SharedAccessKey=m/xwS4LG0uCc6PRNjRMzZn79/aPJLLGVG3uxiFiVmyo=";

var Sp = require("serialport");
var serialport = new Sp("COM11", {
    baudRate: 9600,
    //parser: Sp.parsers.readline('\r\n')
});

var C2D_Cmd;
var client;

serialport.on('open', function () {
    console.log('port open...');
});

function initClient(connectionStringParam) {
    var connectionString = ConnectionString.parse(connectionStringParam);
    //var deviceId = connectionString.DeviceId;

    client = Client.fromConnectionString(connectionStringParam, Protocol);
    return client;
}



function receiveMessageCallback(msg) {
    //    blinkLED();
    console.log(msg);
    C2D_Cmd = JSON.parse(msg.getData());
    var message = msg.getData().toString('utf-8');
    client.complete(msg, function () {
        console.log('Receive message: ' + message);
    });
    console.log(C2D_Cmd.gender);
    console.log(C2D_Cmd.Lock);
    //console.log(C2D_Cmd.R);
    //console.log(C2D_Cmd.G);
    //console.log(C2D_Cmd.B);
    var data = JSON.stringify(C2D_Cmd);
    serialport.write(data, function (err) {
        if (err) {
            return console.log('error on write: ', err.message);
        }
        console.log("sending message: " + data);
    });
}

client = initClient(ConString);

client.open(function (err) {
    if (err) {
        console.error('[IoT hub Client] Connect error: ' + err.message);
        return;
    }
    console.log("client open");

});

client.on('message', receiveMessageCallback);



//function printResultFor(op) {
//  return function printResult(err, res) {
//    if (err) console.log(op + ' error: ' + err.toString());
//    if (res) console.log(op + ' status: ' + res.constructor.name);
//  };
//}


var connectCallback = function (err) {
    if (err) {
        console.log('Could not connect: ' + err);
    } else {
        console.log('Client connected');
    }
};

//function getDateTime() {
//    var date = new Date();
//    var hour = date.getHours();
//    hour = (hour < 10 ? "0" : "") + hour;
//    var min  = date.getMinutes();
//    min = (min < 10 ? "0" : "") + min;
//    var sec  = date.getSeconds();
//    sec = (sec < 10 ? "0" : "") + sec;
//    var month = date.getMonth() + 1;
//    month = (month < 10 ? "0" : "") + month;
//    var day  = date.getDate();
//    day = (day < 10 ? "0" : "") + day;
//    return day +"/"+ hour +":"+ min +":"+ sec;
//}


//serialPort.on('data', function(data) {
//    var jobj = JSON.parse(data);

//    var temperature = jobj.Temperature;
//    var humidity = jobj.Humidity;
//    var heatIndex = jobj.HeatIndex;  
//    var timestamp = getDateTime();          
//    var data = JSON.stringify({ deviceId: 'Linkit7688', temperature: temperature, humidity: humidity, heatIndex:heatIndex, timestamp:timestamp });
//    var message = new Message(data);
//    message.properties.add('temperatureAlert', (temperature > 30) ? 'true' : 'false');
//    //console.log("Sending message: " + message.getData());
// //   client.sendEvent(message, printResultFor('send'));

// //   console.log("JSON[T]=",jobj.Temperature);
// //   console.log("JSON[H]=",jobj.Humidity);
// //   console.log("JSON[HeatIndex]=",jobj.HeatIndex);
//}); 




//client.open(connectCallback);