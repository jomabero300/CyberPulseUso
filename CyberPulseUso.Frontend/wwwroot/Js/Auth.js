window.cyberPulseAuth = {
    request: async function (url, method, body) {

        const sendRequest = async () => {
            const options = {
                method: method,
                credentials: "include"
            };

            if (body !== null && body !== undefined) {
                options.headers = {
                    "Content-Type": "application/json"
                };

                options.body = JSON.stringify(body);
            }

            const response = await fetch(url, options);
            const responseBody = await response.text();

            return {
                ok: response.ok,
                status: response.status,
                body: responseBody,
                error: null
            };
        };

        try {
            const requestUrl = new URL(url, window.location.href);

            const isRefresh = requestUrl.pathname
                .toLowerCase()
                .endsWith("/api/accounts/refresh");

            if (isRefresh && navigator.locks?.request) {
                return await navigator.locks.request(
                    "cyberpulse-refresh-token",
                    async () => await sendRequest()
                );
            }

            return await sendRequest();
        }
        catch (error) {
            console.error("Error during fetch:", error);

            return {
                ok: false,
                status: 0,
                body: null,
                error: error.message
            };
        }
    }
};