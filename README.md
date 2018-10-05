# Documate
Documate is a document publishing platform backed by an ethereum blockchain. There are 3 main components to documate
1. Server
2. Blockchain
3. Client App

## 1. Server
The server is used to actually host the documents. It is a *ASP.NET Core MVC* application exposing REST apis to securely store and retrieve document data. 

## 2. Blockchain
The blockchain host smart contracts which store access and permission information for a document.

## 3. Client
It is a react native application able to communicate with the server to create, issue, present, publish and view documents.

### Roles

Three kinds of roles are supported by documate
1. Creator - Can create a document. A creator can view the document
1. Issuer - Creator can issue a document to one issuee. Creator can also revoke an issuance. An issue can present a document.
1. Presenter - creator or issuee can present document to any number of presentees. A presentation can be revoked after which presentee cannot view the document.
1. Publisher - Owner or publishee can publish a document. This is a transitive relationship.


![Workflow](/docs/workflow.png)
