const { Client, LocalAuth } = require("whatsapp-web.js");
const qrcode = require("qrcode-terminal");

const client = new Client({
    authStrategy: new LocalAuth(),   // 👈 Session save hoga
    puppeteer: {
        headless: false,             // WhatsApp visible rahe
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    }
});

client.on("qr", (qr) => {
    qrcode.generate(qr, { small: true });
    console.log("📌 Please Scan QR ");
});

client.on("ready", () => {
    console.log("💥 WhatsApp Client Ready!");

    const message = process.argv[2];
    const number = process.argv[3];

    if (!message || !number) {
        console.log("⚠ Usage: node sendMessage.js \"Message\" 916XXXXXXXX");
        return;
    }

    const finalNumber = `${number}@c.us`;

    client.sendMessage(finalNumber, message)
        .then(() => {
            console.log("📨 Message Sent Successfully!");
        })
        .catch((err) => {
            console.log("❌ Send Error:", err);
        });
});

client.initialize();
