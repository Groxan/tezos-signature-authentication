window.signOut = async (dapp, network) => {
    const client = beacon.getDAppClientInstance({
        name: dapp,
        preferredNetwork: network
    });
    client.destroy();
    submitForm('/auth/signout',
        ['redirectUrl', window.location.pathname]);
};

window.signIn = async (dapp, network, payload, timestamp) => {
    const { pubkey, signature } = await signAuthMessage(dapp, network, payload);
    if (pubkey && timestamp && signature) {
        submitForm('/auth/signin',
            ['publicKey', pubkey],
            ['signature', signature],
            ['timestamp', timestamp],
            ['redirectUrl', window.location.pathname]);
    }
};

async function signAuthMessage(dapp, network, payload) {
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
            const res = await client.requestSignPayload({
                signingType: beacon.SigningType.MICHELINE,
                payload: payload
            });
            if (res.signature) {
                return ({
                    pubkey: accountInfo.publicKey,
                    signature: res.signature
                });
            }
        }
    }
    catch (error) {
        console.error(error);
    }
    return ({
        pubkey: null,
        signature: null
    });
}

function submitForm(action, ...inputs) {
    const form = document.createElement('form');
    form.setAttribute('method', 'post');
    form.setAttribute('action', action);
    inputs.forEach(([n, v]) => form.appendChild(hiddenInput(n, v)));
    document.body.appendChild(form);
    form.submit();
}

function hiddenInput(name, value) {
    const input = document.createElement('input');
    input.setAttribute('type', 'hidden');
    input.setAttribute('name', name);
    input.setAttribute('value', value);
    return input;
}
