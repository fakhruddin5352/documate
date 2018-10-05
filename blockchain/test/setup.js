var abi = require('ethereumjs-abi')

const Sharer = artifacts.require('Sharer');
const Documate = artifacts.require('Documate');

module.exports = async (accounts, sender = accounts[0], server = accounts[1] ) => {
    const publisher = await Sharer.new();
    const presenter = await Sharer.new();
    const issuer = await Sharer.new();

    const documate = await Documate.new(publisher.address,presenter.address, issuer.address,  server);

    let obj = {server, sender, publisher, documate, presenter, issuer};
    obj.serverSign = (data, _sender = sender, _server = server, _contract = documate.address) => {
        var hash = '0x' + abi.soliditySHA3(
            ['bytes32', 'address', 'address'],
            [data, _sender, _contract]
        ).toString('hex');
        return web3.eth.sign( _server, hash);
    };
    return obj;

};