
import "dotenv/config";

const SteamUser = require("steam-user");
const SteamCommunity = require("steamcommunity");
const TradeOfferManager = require("steam-tradeoffer-manager");
const readline = require("node:readline");

const username = process.env.STEAM_USERNAME;
const password = process.env.STEAM_PASSWORD;

if (!username || !password) {
    console.error("Configure STEAM_USERNAME e STEAM_PASSWORD no .env");
    process.exit(1);
}

const steam = new SteamUser({
    dataDirectory: "./data"
});

const community = new SteamCommunity();
const manager = new TradeOfferManager({ steam, community, language: "en" });
manager.on("newOffer", (offer: any) => {
    console.log("\n========================================");
    console.log("NOVA OFERTA RECEBIDA!");
    console.log("========================================");

    console.log("Oferta ID:", offer.id);
    console.log("Remetente:", offer.partner.getSteamID64());

    const itensEnviados = offer.itemsToReceive || [];
    const itensSolicitados = offer.itemsToGive || [];

    console.log("\nItens oferecidos:", itensEnviados.length);
    console.log("Itens solicitados do bot:", itensSolicitados.length);

    for (const item of itensEnviados) {
        console.log("----------------------------------------");
        console.log("Nome:", item.market_hash_name || item.name);
        console.log("App ID:", item.appid);
        console.log("Context ID:", item.contextid);
        console.log("Asset ID:", item.assetid);
    }

    if (itensSolicitados.length > 0) {
        console.log("\n[RECUSADA NA ANÁLISE] A oferta solicita itens do bot.");
        return;
    }

    if (itensEnviados.length === 0) {
        console.log("\n[RECUSADA NA ANÁLISE] A oferta está vazia.");
        return;
    }

    const todosSaoCS2 = itensEnviados.every(
        (item: any) =>
            String(item.appid) === "730" &&
            String(item.contextid) === "2"
    );

    if (!todosSaoCS2) {
        console.log("\n[RECUSADA NA ANÁLISE] Existem itens que não são de CS2.");
        return;
    }

    console.log("\n[APROVADA NA ANÁLISE]");
    console.log("Oferta válida para depósito.");
    console.log("Nenhuma troca foi aceita automaticamente.");

    console.log("========================================\n");
});

console.log("Iniciando Steam Bot...");

steam.on("connecting", () => {
    console.log("Estabelecendo conexão...");
});

steam.on("connected", () => {
    console.log("Conexão com os servidores Steam estabelecida.");
});

steam.on("loggedOn", () => {
    console.log("\nLogin realizado com sucesso!");
    console.log("SteamID64:", steam.steamID.getSteamID64());
});

steam.on(
    "steamGuard",
    (
        domain: string | null,
        callback: (code: string) => void,
        lastCodeWrong: boolean
    ) => {
        if (lastCodeWrong) {
            console.log("Código anterior incorreto. Tente novamente.");
        }

        if (domain) {
            console.log(`Código enviado para o e-mail terminado em ${domain}`);
        } else {
            console.log("Código Steam Guard Mobile solicitado.");
        }

        const terminal = readline.createInterface({
            input: process.stdin,
            output: process.stdout
        });

        terminal.question("Digite o código Steam Guard: ", (code: string) => {
            terminal.close();

            callback(code.trim());

            console.log("Código enviado para autenticação...");
        });
    }
);

steam.on("webSession", (_sessionId: string, cookies: string[]) => {
    console.log("\nSessão web Steam iniciada.");
    console.log("Configurando sessão do gerenciador...");

    manager.setCookies(cookies, (err: Error | null) => {
        if (err) {
            console.error("Erro ao configurar sessão:", err.message);
            return;
        }

        console.log("Sessão do gerenciador configurada.");

        consultarInventario();
    });
});

steam.on("error", (err: Error) => {
    console.error("\nErro Steam:", err.message);
});

steam.on("disconnected", (eresult: number, message: string) => {
    console.log("\nDesconectado da Steam:");
    console.log("Código:", eresult);
    console.log("Mensagem:", message);
});

function consultarInventario() {
    console.log("\nConsultando inventário de CS2...");

    manager.getInventoryContents(
        730,
        2,
        false,
        (err: Error | null, inventory: any[]) => {
            if (err) {
                console.error("Erro ao consultar inventário:", err.message);
                return;
            }

            console.log(`\nItens encontrados: ${inventory.length}\n`);

            for (const item of inventory) {
                console.log("--------------------------------");
                console.log("Nome:", item.market_hash_name);
                console.log("Asset ID:", item.assetid);
                console.log("Class ID:", item.classid);
                console.log("Instance ID:", item.instanceid);
                console.log("Negociável:", item.tradable);
            }

            console.log("\nConsulta finalizada.");
        }
    );
}

console.log("Iniciando conexão com a Steam...\n");

steam.logOn({
    accountName: username,
    password: password
});
