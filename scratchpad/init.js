const _Publisher = require('../blockchain/build/contracts/Publisher.json');
const _Documate = require('../blockchain/build/contracts/Documate.json');
const _Document = require('../blockchain/build/contracts/Document.json');

const sharedFolder = '../shared';
const fs = require('fs');
let Web3 = require("web3");

var web3 = new Web3(
    new Web3.providers.HttpProvider("http://localhost:7545")
);

let Publisher = web3.eth.contract(_Publisher.abi);
let Documate = web3.eth.contract(_Documate.abi);


const server = web3.eth.accounts[1];
let publisher, documate;
Publisher.new({ from: web3.eth.accounts[0], gas: 6700000, data: _Publisher.bytecode }, (e, contract) => {
    if (contract.address) {
        publisher = contract;
        Documate.new(publisher.address, server, { from: web3.eth.coinbase, gas: 6700000, data: _Documate.bytecode }, (e, contract) => {
            if (e)
                return console.error(e);

            if (contract.address) {
                documate = contract;

                var obj = {
                    Contracts: {
                        Publisher: { ABI: JSON.stringify(_Publisher.abi), Address: publisher.address },
                        Document: { ABI: JSON.stringify(_Document.abi) },
                        Documate: { ABI: JSON.stringify(_Documate.abi), Address: documate.address },
                    }
                }
                fs.writeFileSync(`${sharedFolder}/settings.json`, JSON.stringify(obj));
                console.log('Publisher=%s', publisher.address);
                console.log('Documate=%s', documate.address);
            }
        });
    }
});
// const documate = await ;
//return documate.address;
