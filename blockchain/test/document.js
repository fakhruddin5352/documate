const Document = artifacts.require('Document');
const setup = require('./setup');

contract('Document', accounts => {

    let context;
    beforeEach(async () => {
        context = await setup(accounts);
    });


    const createDocument = async (name) => {
        const document = web3.sha3(name);
        var sig = context.serverSign(document);
        const receipt = await context.documate.createDocument(name, document, sig);
        const documentCreateEvent = receipt.logs.find(x => x.event == 'DocumentCreated');
        const documentContract = Document.at(documentCreateEvent.args.document);
        return documentContract;
    };

    let document;

    beforeEach(async () => {
        document = await createDocument("Document1");
    });

    it('should be set to issued after issuane', async () => {
        
        const issuee = accounts[2];
        await document.issue(issuee);
        assert.isTrue((await document.isIssued.call()), 'document should be issued');
        assert.equal((await document.getIssuedTo.call()), issuee, 'document should be issued to the provided issuer');
    });


    it('should fail to issue if already published', async () => {
        
        const issuee = accounts[2];
        await document.publish([issuee]);
        assert.isTrue((await document.isPublished.call()), 'document should be published');
        try {
            await document.issue(issuee);
        } catch (err) {
            assert(/document_is_published/.test(err), 'issue should revert by throwing "document_is_published" if already published');
            return;
        }
        assert.fail('issue should fail if document already published');
    });

    it('should fail to issue if issuer is not owner', async () => {
        
        const issuee = accounts[2];
        try {
            await document.issue(issuee, {
                from: accounts[3]
            });
        } catch (err) {
            assert(/issuer_is_not_owner/.test(err), 'issue should revert by throwing "issuer_is_not_owner" if issuer is not owner');
            return;

        }
        assert.fail('issue should fail if issuer is not owner');
    });


    it('should fail to issue if issuee is owner', async () => {
        
        const issuee = context.sender;
        try {
            await document.issue(issuee);
        } catch (err) {
            assert(/issuee_is_owner/.test(err), 'issue should revert by throwing "issuee_is_owner" if issuee is owner');
            return;

        }
        assert.fail('issue should fail if issuee is owner');
    });


    it('should be set to presented after presentation', async () => {
        
        const presentee = accounts[2];
        await document.present([presentee]);
        assert.isTrue((await document.isPresented.call()), 'document should be presented');
        assert.include((await document.getPresentedBy.call(context.sender)), presentee, 'document should be presented to the provided presentee');

        const presentees = [presentee, accounts[3]];
        await document.present(presentees);
        assert.deepEqual((await document.getPresentedBy.call(context.sender)), presentees, 'document should be presented to the provided presentees');

    });

    it('should fail to present if not issued or owned', async () => {
        
        const presentee = accounts[2];
        assert.notEqual((await document.getIssuedTo.call()), context.sender, 'document is issued to sender');
        assert.equal((await document.owner.call()), context.sender, 'document is owned by sender');
        try {
            await document.present([presentee], {
                from: accounts[3]
            });
        } catch (err) {
            assert(/document_is_not_issued_or_owned/.test(err), 'present should revert by throwing "document_is_not_issued_or_owned" if not issued or owned ');
            return;
        }
        assert.fail('present should fail if document is not owner or issued');
    });

    it('should present if issued not owned', async () => {
        
        const presentee = accounts[2];
        const caller = accounts[3];
        await document.issue(caller);

        assert.equal((await document.getIssuedTo.call()), caller, 'document is not issued to sender');
        assert.notEqual((await document.owner.call()), caller, 'document is owned by sender');
        await document.present([presentee], {
            from: caller
        });
        assert.isTrue((await document.isPresentedBy.call(caller)), 'document should be presented');
        assert.include((await document.getPresentedBy.call(caller)), presentee, 'document should be presented to the provided presentee');
    });

    it('should present if owned not issued', async () => {
        
        const presentee = accounts[2];
        const caller = accounts[0];
        await document.issue(accounts[3]);

        assert.notEqual((await document.getIssuedTo.call()), caller, 'document is issued to sender');
        assert.equal((await document.owner.call()), caller, 'document is not owned by sender');
        await document.present([presentee], {
            from: caller
        });
        assert.isTrue((await document.isPresented.call()), 'document should be presented');
        assert.include((await document.getPresentedBy.call(caller)), presentee, 'document should be presented to the provided presentee');
    });

    it('should not be presented if given 0 presentees', async () => {
        
        const presentees = [];
        await document.present(presentees);
        assert.isFalse((await document.isPresented.call()), 'document should be not be set to presented');
    });

    it('should be set to published after publication', async () => {
        
        const publishee = accounts[2];
        await document.publish([publishee]);
        assert.isTrue((await document.isPublished.call()), 'document should be published');

    });

    it('should be not be set to published if 0 publishees', async () => {
        
        await document.publish([]);
        assert.isFalse((await document.isPublished.call()), 'document should not be published');
    });

    it('should fail to publish if already issued', async () => {
        
        await document.issue(accounts[2]);
        assert.isTrue((await document.isIssued.call()), 'document should be issued');

        try {
            await document.publish([accounts[3]]);
        } catch (err) {
            assert(/document_is_issued/.test(err), 'publish should revert by throwing "document_is_issued" if issued');
            return;
        }
        assert.fail('document should not be published');
    });

    it('should fail to publish if already presented', async () => {
        
        await document.present([accounts[2]]);
        assert.isTrue((await document.isPresented.call()), 'document should be presented');

        try {
            await document.publish([accounts[3]]);
        } catch (err) {
            assert(/document_is_presented/.test(err), 'publish should revert by throwing "document_is_presented" if presented');
            return;
        }
        assert.fail('document should not be published');
    });

    it('should fail to publish if not owned or published to', async () => {
        const publisher = accounts[2];
        
        assert.notEqual((await document.owner.call()), publisher, 'document should not be owned by account2');
        await document.publish([accounts[3]]);
        assert.isTrue((await document.isPublished.call()), 'document should be published');
        assert.isFalse((await context.publisher.isDocumentSharedTo.call(document.address, publisher)), 'document should not already be published to account2');

        try {
            await document.publish([accounts[3]], {
                from: publisher
            });
        } catch (err) {
            assert(/document_is_not_issued_or_owned/.test(err), 'publish should revert by throwing "document_is_not_issued_or_owned" if not owned or published');
            return;
        }
        assert.fail('document should not be published');
    });

    it('should publish if owned not published to', async () => {
        const publisher = accounts[2];
        
        assert.equal((await document.owner.call()), context.sender, 'document should be owned');
        assert.isFalse((await document.isPublished.call()), 'document should not be published');
        await document.publish([accounts[3]]);
        assert.isTrue((await context.publisher.isDocumentSharedTo.call(document.address, accounts[3])), 'document should  be published');
    });

    it('should publish if not owned but published to', async () => {
        const publisher = accounts[2];
        
        assert.notEqual((await document.owner.call()), publisher, 'document should not be owned');

        await document.publish([publisher]);
        assert.isTrue((await context.publisher.isDocumentSharedTo.call(document.address, publisher)), 'document should  be published');

        await document.publish([accounts[3]], {
            from: publisher
        });
        assert.isTrue((await context.publisher.isDocumentSharedTo.call(document.address, accounts[3])), 'document should  be published');
    });

    it('should revoke presentation', async() => {
        await document.present([accounts[3],accounts[4]]);
        assert.isTrue((await document.isPresented.call()), 'document should be presented');
        assert.include((await document.getPresentedBy.call(context.sender)), accounts[3], 'document should be presented to be accounts3');
        assert.include((await document.getPresentedBy.call(context.sender)), accounts[4], 'document should be presented to be accounts4');
        await document.revokePresentation([accounts[3]]);
        assert.notInclude((await document.getPresentedBy.call(context.sender)) , accounts[3], 'document should not revoked from accounts3');
        assert.include((await document.getPresentedBy.call(context.sender)), accounts[4], 'document should be presented to be accounts4');

    });


    it('should revoke issuance', async() => {
        await document.issue(accounts[3]);
        assert.isTrue((await document.isIssued.call()), 'document should be issued');
        assert.equal((await document.getIssuedTo.call()), accounts[3], 'document should be issued to be accounts3');
        await document.revokeIssuance();
        assert.isFalse((await document.isIssued.call()) , 'document should be be issued');
        assert.equal((await document.getIssuedTo.call()), 0, 'issuedTo should be set to 0');

    });

    it('should revoke presentations of issuee on revoke issuance', async() => {
        const issuee = accounts[3];
        await document.issue(issuee);
        assert.isTrue((await document.isIssued.call()), 'document should be issued');
        assert.equal((await document.getIssuedTo.call()), issuee, 'document should be issued to be accounts3');
        await document.present([accounts[4]], {
            from: issuee
        });                
        await document.revokeIssuance();
        assert.isFalse((await document.isIssued.call()) , 'document should be be issued');
        assert.equal((await document.getIssuedTo.call()), 0, 'issuedTo should be set to 0');
        assert.isFalse((await document.isPresentedBy.call(issuee)) , 'document should not be presented');

    });


    it('should not revoke issuance if not owner', async () => {
        await document.issue(accounts[3]);
        await document.revokeIssuance({
            from: accounts[2]
        });
        assert.equal((await document.getIssuedTo.call()),accounts[3] );

    });

    it('should be viewable if owned', async () => {
        const canView = await document.canView.call(context.sender);
        assert.isTrue(canView, 'document should be viewed if owned');
    });
    it('should be viewable if issued', async () => {
        const issuee = accounts[2];
        await document.issue(issuee);
        const canView = await document.canView.call(issuee);
        assert.isTrue(canView, 'document should be viewed if issued');
    });
    it('should be viewable if published', async () => {
        const publishee = accounts[2];
        await document.publish([publishee]);
        const canView = await document.canView.call(publishee);
        assert.isTrue(canView, 'document should be viewed if published');
    });
    it('should be viewable if presented', async () => {
        const presentee = accounts[2];
        await document.issue(presentee);
        const canView = await document.canView.call(presentee);
        assert.isTrue(canView, 'document should be viewed if presented');
    });
    it('should not be viewable if either not owner or issued or presented or published', async () => {
        assert.notEqual((await document.getIssuedTo.call(),accounts[2]));
        assert.notEqual((await document.owner.call(),accounts[2]));
        assert.isFalse((await document.isPresentedTo.call(accounts[2])));
        assert.isFalse((await context.publisher.isDocumentSharedTo.call(document.address, accounts[2])));
        const canView = await document.canView.call(accounts[2]);
        assert.isFalse(canView, 'document should be viewed if presented');
    });
    
});