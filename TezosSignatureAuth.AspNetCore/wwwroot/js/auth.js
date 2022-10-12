async function signIn(e, dapp, network) {
    const { pubkey, signature, timestamp } = await signAuthMessage(dapp, network);
    if (pubkey && signature && timestamp) {
        e.target['PublicKey'].value = pubkey;
        e.target['Signature'].value = signature;
        e.target['Timestamp'].value = timestamp;
        e.target['RedirectUrl'].value = window.location.pathname;
        e.target.submit();
    }
};

function signOut(e) {
    e.target['RedirectUrl'].value = window.location.pathname;
};

async function signAuthMessage(dapp, network) {
    try {
        const client = beacon.getDAppClientInstance({
            name: dapp,
            preferredNetwork: network
        });

        let accountInfo = await client.getActiveAccount();
        if (!accountInfo || accountInfo.network.type !== network) {
            const res = await client.requestPermissions({
                scope: [
                    beacon.PermissionScope.SIGN
                ],
                network: {
                    type: network
                }
            });
            accountInfo = res.accountInfo;
        }

        if (accountInfo) {
            const timestamp = utcNow();
            const res = await client.requestSignPayload({
                signingType: beacon.SigningType.MICHELINE,
                payload: pack(`Tezos Signed Message: Hi there, ${timestamp}!`)
            });
            if (res.signature) {
                return ({
                    pubkey: accountInfo.publicKey,
                    signature: res.signature,
                    timestamp: timestamp.replace(' ', 'T') + 'Z'
                });
            }
        }
    }
    catch (error) {
        console.error(error);
    }
    return ({
        pubkey: null,
        signature: null,
        timestamp: null
    });
}

function pack(msg) {
    return `0501${intToHex(msg.length)}${asciiToHex(msg)}`;
}

function intToHex(v) {
    let res = '';
    for (let i = 3; i >= 0; i--, v >>= 8) {
        res = (v & 0xFF).toString(16).padStart(2, '0') + res;
    }
    return res;
}

function asciiToHex(s) {
    let res = '';
    for (let i = 0; i < s.length; i++) {
        res += s.charCodeAt(i).toString(16).padStart(2, '0');
    }
    return res;
}

function utcNow() {
    const dt = new Date();
    return dt.getUTCFullYear() +
        '-' + pad2(dt.getUTCMonth() + 1) +
        '-' + pad2(dt.getUTCDate()) +
        ' ' + pad2(dt.getUTCHours()) +
        ':' + pad2(dt.getUTCMinutes()) +
        ':' + pad2(dt.getUTCSeconds());
}

function pad2(n) {
    return n < 10 ? ('0' + n) : n;
}

