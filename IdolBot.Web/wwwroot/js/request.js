function axiosCreate() {
    var config = {
        'Content-Type': 'application/json',
        timeout: 10 * 1000,
        headers: {
            'request-Type': 'json'//json-返回数据，view-返回视图/页面
        }
    }
    const instance = axios.create(config);
    instance.interceptors.response.use(
        response => {
            if (response.status === 200) {
                return Promise.resolve(response); //进行中        
            } else {
                return Promise.reject(response); //失败       
            }
        },
        error => {
            if (error.response?.status && error.response?.status != 200) {
                if (error.response.status === 500) {
                    window?.ElementPlus?.ElMessage({
                        message: error.response.data.msg,
                        type: 'error'
                    });
                } else if (error.response.status === 401) {
                    window?.ElementPlus?.ElMessage({
                        message: "身份认证失败！请重新登录！",
                        type: 'error',
                        onClose: close()
                    });
                } else if (error.response.status === 403) {
                    window?.ElementPlus?.ElMessage({
                        message: "API未授权！",
                        type: 'error'
                    });
                } else if (error.response.status === 404) {
                    window?.ElementPlus?.ElMessage({
                        message: "API路径错误！",
                        type: 'error'
                    });
                } else if (error.response.status === 502) {
                    window?.ElementPlus?.ElMessage({
                        message: "请求错误！",
                        type: 'error'
                    });
                } else {
                    window?.ElementPlus?.ElMessage({
                        message: "未知错误！",
                        type: 'error'
                    });
                }
            } else {
                window?.ElementPlus?.ElMessage({
                    message: error.message,
                    type: 'error'
                });
            }
            return Promise.reject(error);
        }
    );
    function close() {
        window.location.href = '/home/login'
    }
    return instance
}
var axiosInstance = axiosCreate();
var httpClient = {
    instance: axiosInstance,
    get: async function (url, params, headers) {
        let getConfig = {}
        if (params) {
            getConfig.params = params;
        }
        if (headers) {
            getConfig.headers = headers;
        }
        let res = await axiosInstance.get(url, getConfig)
        return res.data
    },

    post: async function (url, data, headers) {
        let postConfig = {}
        if (headers) {
            postConfig.headers = headers;
        }
        let res = await axiosInstance.post(url, data, postConfig)
        return res.data
    }
}