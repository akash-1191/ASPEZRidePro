const venom = require('venom-bot');
const path = require('path');

const message = process.argv[2];
const number = process.argv[3];

venom.create({
    session: 'ezride-session',
    headless: 'new',//if user not login then set frist false the set the 'new' frist delete the token
    useChrome: true,
    debug: false,
    logQR: true,
    browserPath: 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe',
    autoClose: true, // don't close browser automatically the is set the false (if use 'new' the set is false othewise true)
    disableSpins: true,
    updatesLog: false,
})
    .then((client) => {
        return client.sendText(`${number}@c.us`, message)
            .then(() => {
                console.log(' Message sent successfully!');
                //  Let venom save session properly, then exit (remove frist time to logint then)
                setTimeout(() => {
                    console.log(' Exiting now...');
                    process.exit(0);
                }, 5000); 
            })
            .catch((error) => {
                console.error(' Error sending message:', error);
                setTimeout(() => process.exit(1), 5000);
            });
    })
    .catch((error) => {  
        console.error(' Venom error:', error);
        setTimeout(() => process.exit(1), 5000);
    });
