const fs = require('fs');

const documate = fs.readFileSync('../blockchain/build/contracts/Documate.json','utf8');
var contract = JSON.parse(documate);
console.log(JSON.stringify(contract.abi).replace(/\"/g,"'"));

const publisher = fs.readFileSync('../blockchain/build/contracts/Publisher.json','utf8');
var contract = JSON.parse(publisher);
console.log(JSON.stringify(contract.abi).replace(/\"/g,"'"));
