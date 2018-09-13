let Web3 = require('web3');
const fs = require('fs');
const request = require('request');
const settings = require('../shared/settings.json').Contracts;
console.log(settings);
const _Publisher = JSON.parse(settings.Publisher.ABI);
const _Documate = JSON.parse(settings.Documate.ABI);


var web3 = new Web3(
    new Web3.providers.HttpProvider('http://localhost:7545')
);

let Publisher = web3.eth.contract(_Publisher);
let Documate = web3.eth.contract(_Documate);

const file = fs.openSync('/Users/fakhruddin/Downloads/img1.jpeg', 'r');
const data = fs.readFileSync(file);
var base64 = data.toString('base64');
var dochash = web3.sha3(base64);
const signature = web3.eth.sign(web3.eth.accounts[0], dochash);

const uploadOptions = {
    method: 'POST',
    uri: 'http://localhost:5000/api/document',
    formData: {
        file: fs.createReadStream('/Users/fakhruddin/Downloads/img1.jpeg'),
        signature: signature,
        sender: web3.eth.accounts[0],
        name: 'img1.jpeg'
    }
};

const downloadOptions = {
    method: 'GET',
    url: 'http://localhost:5000/api/document',
    qs: {
        signature: signature,
        sender: web3.eth.accounts[0],
        hash: dochash
    }
};

const publisher = Publisher.at(settings.Publisher.Address);
const documate = Documate.at(settings.Documate.Address);

var events = documate.allEvents();
// watch for changes
events.watch(function (error, event) {
    if (!error)
        if (event.event == 'DocumentCreated') {
            console.log('Document address ', event.args.document);
            request(downloadOptions).pipe(fs.createWriteStream('file.jpeg'));

        }
});


request(uploadOptions, (err, response, body) => {
    if (err)
        return console.log('Error ', err);

    console.log('Account ', web3.eth.accounts[0]);
    console.log('Hash ', dochash);
    console.log('User signature ', signature);
    var json = JSON.parse(response.body);
    console.log('Server signature ', json.signature);
    console.log(documate.getSigner.call(dochash, json.signature), documate.serverAddress.call());

    documate.createDocument("Document1", dochash, json.signature, { gas: 6721975, from: web3.eth.coinbase }, (e, receipt) => {
        if (e) {
            return console.log(e);
        }
        console.log(receipt);
        if (receipt.logs) {
        }
    });
    //;
});
