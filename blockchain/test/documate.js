var abi = require('ethereumjs-abi')

let Documate = artifacts.require("Documate");
let Publisher = artifacts.require("Publisher");


contract("Documate", accounts => {
    let serverAccount = accounts[1];
    let documate;
    let publisher;

    const serverSign = (document, owner) => {
        var hash = "0x" + abi.soliditySHA3(
            ['bytes32', 'address', 'address'],
            [document, owner, documate.address]
        ).toString('hex');
        return web3.eth.sign( serverAccount, hash);
    };
    
    beforeEach(async () => {
        publisher = await Publisher.new();
        documate = await Documate.new(publisher.address, serverAccount);
    });

    it("test signature", async () => {
        let ownerAccount = accounts[0];
        let documentId = web3.sha3("0x1");
        var sig = serverSign(documentId, ownerAccount);
        var test = await documate.testSignature(documentId, sig);
        assert.equal(test, serverAccount, "should be equal");
    });

    
    it("should create a document", async () => {
        let ownerAccount = accounts[0];
        let documentId = web3.sha3("0x1");
        var sig = serverSign(documentId ,ownerAccount);
        var receipt = await documate.createDocument("Document1", documentId, sig);
        let documentCreateEvent = receipt.logs.find(x => x.event == 'DocumentCreated' );
        assert.exists(documentCreateEvent, "DocumentCreated Event not returned in transaction log");    
        assert.exists(documentCreateEvent.args, "DocumentCreated Event has no arguments");
        assert.exists(documentCreateEvent.args.document, "DocumentCreated Event does not have document arguments");
    
    });

    it("should fail on invalid server signature", async () => {
        let ownerAccount = accounts[0];
        let documentId = web3.sha3("0x1");
        var sig = serverSign(documentId ,ownerAccount);
        try
        {
            var receipt = await documate.createDocument("Document1", web3.sha3("0x2"), sig);
            assert(false,'Invalid signature passed');
        }catch(err){
            assert(/Invalid server signature/.test(err),'Invalid server signature error not thrown');
        }
    });
});