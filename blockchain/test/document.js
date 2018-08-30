const Document = artifacts.require('Document');
const setup = require('./setup');

contract('Document', accounts => {

    let context;
    beforeEach(async () => {
        context = await setup(accounts);
    });


    const createDocument  = async (name) => {
        const document = web3.sha3(name);
        var sig = context.serverSign( document);
        const receipt = await context.documate.createDocument(name, document, sig);
        const documentCreateEvent = receipt.logs.find(x => x.event == 'DocumentCreated' );
        const documentContract = Document.at(documentCreateEvent.args.document);
        return documentContract;
    };

    it('should be set to issued after issuane', async () => {
        const document = await createDocument("Document1");
        const issuee = accounts[2];
        await document.issue(issuee);
        assert.equal((await document.issued.call()), true, 'document should be issued' );
        assert.equal((await document.issuedTo.call()), issuee, 'document should be issued to the provided issuer' );        
    });


    it('should fail to issue if already published', async () => {
        const document = await createDocument("Document1");
        const issuee = accounts[2];
        await document.publish([issuee]);
        assert.equal((await document.published.call()), true, 'document should be published');
        try {
            await document.issue(issuee);
        } catch(err) {
            assert(/document_is_published/.test(err),'issue should revert by throwing "document_is_published" if already published');
            return;
        }
        assert(false,'issue should fail if document already published');            
    });

    it('should fail to issue if issuer is not owner', async () => {
        const document = await createDocument("Document1");
        const issuee = accounts[2];
        try {
            await document.issue(issuee,{
                from: accounts[3]
            });
        } catch (err) {
            assert(/owner_is_not_issuer/.test(err),'issue should revert by throwing "owner_is_not_issuer" if issuer is not owner');
            return;

        }
        assert(false,'issue should fail if issuer is not owner');            
    });


    it('should fail to issue if issuee is owner', async () => {
        const document = await createDocument("Document1");
        const issuee = context.sender;
        try {
            await document.issue(issuee);
        } catch (err) {
            assert(/issuee_is_owner/.test(err),'issue should revert by throwing "issuee_is_owner" if issuee is owner');
            return;

        }
        assert(false,'issue should fail if issuee is owner');            
    });


    it('should be set to presented after presentation', async () => {
        const document = await createDocument("Document1");
        const presentee = accounts[2];
        await document.present([presentee]);
        assert.equal((await document.presented.call()), true, 'document should be presented' );
        assert.equal((await document.presentedTo.call(0)), presentee, 'document should be presented to the provided presentee' );        
    });

    it('should fail to present if not issued or owned', async () => {
        const document = await createDocument("Document1");
        const presentee = accounts[2];
        assert.notEqual((await document.issuedTo.call()), context.sender, 'document is issued to sender' );
        assert.equal((await document.owner.call()), context.sender, 'document is owned by sender' );
        try {
                await document.present([presentee], {
                    from: accounts[3]
                });
        } catch(err) {
            assert(/document_is_not_issued_or_owned/.test(err),'present should revert by throwing "document_is_not_issued_or_owned" if not issued or owned ');
            return;
        }
        assert(false,'present should fail if document is not owner or issued');            
    });

    it('should present if issued not owned', async () => {
        const document = await createDocument("Document1");
        const presentee = accounts[2];
        const caller = accounts[3];
        await document.issue(caller);

        assert.equal((await document.issuedTo.call()), caller, 'document is not issued to sender' );
        assert.notEqual((await document.owner.call()), caller, 'document is owned by sender' );
        await document.present([presentee], {
            from: caller
        });
        assert.equal((await document.presented.call()), true, 'document should be presented' );
        assert.equal((await document.presentedTo.call(0)), presentee, 'document should be presented to the provided presentee' );        
    });

    it('should present if owned not issued', async () => {
        const document = await createDocument("Document1");
        const presentee = accounts[2];
        const caller = accounts[0];
        await document.issue(accounts[3]);

        assert.notEqual((await document.issuedTo.call()), caller, 'document is issued to sender' );
        assert.equal((await document.owner.call()), caller, 'document is not owned by sender' );
        await document.present([presentee], {
            from: caller
        });
        assert.equal((await document.presented.call()), true, 'document should be presented' );
        assert.equal((await document.presentedTo.call(0)), presentee, 'document should be presented to the provided presentee' );        
    });
    
});