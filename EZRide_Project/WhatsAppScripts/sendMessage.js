const venom = require('venom-bot');
const path = require('path');

const message = process.argv[2];
const number = process.argv[3];

venom
  .create({
    session: 'ezride-session',
      headless: 'new',
    useChrome: true,
      debug: false,
      logQR: false,
    browserPath: 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe',
    sessionDataPath: path.join(__dirname, 'tokens'),
    autoClose: false, // important: don't close browser = session safe
    disableSpins: true,
      updatesLog: false,
  })
    .then((client) => {
        return client.sendText(`${number}@c.us`, message)
            .then(() => {
                console.log(' Message sent successfully!');
                process.exit(0);
            })
            .catch((error) => {
                console.error(' Error sending message:', error);
                process.exit(1);
            });
    })
    .catch((error) => {
        console.error('Venom error:', error);
        process.exit(1);
    });
