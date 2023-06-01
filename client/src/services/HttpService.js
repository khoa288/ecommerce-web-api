class HttpService {
    constructor() {
        this.setLoading = async () => {};
    }

    setSetLoadingFunction(setLoadingFunction) {
        this.setLoading = setLoadingFunction;
    }

    async post(url, options) {
        try {
            await this.setLoading(true);
            const response = await fetch(url, {
                ...options,
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
            });
            return response;
        } finally {
            await this.setLoading(false);
        }
    }

    async get(url, options) {
        try {
            await this.setLoading(true);
            const response = await fetch(url, {
                ...options,
                method: "GET",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
            });
            return response;
        } finally {
            await this.setLoading(false);
        }
    }

    async delete(url, options) {
        try {
            await this.setLoading(true);
            const response = await fetch(url, {
                ...options,
                method: "DELETE",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
            });
            return response;
        } finally {
            await this.setLoading(false);
        }
    }
}

export default HttpService;
