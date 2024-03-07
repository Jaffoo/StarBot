import axios from 'axios';
import type { AxiosResponse } from 'axios';
import type { Config, ApiResult } from '@/class/model';

const _port = sessionStorage.getItem("HttpPort") || 6051;
const _baseUrl = `http://127.0.0.1:${_port}/api/v1`;

export const startBot = async () => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/startbot`);
    return response.data as ApiResult;
}

export const getConfig = async () => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/getconfig`);
    return response.data as ApiResult;
}

export const saveConfig = async (config: Config) => {
    const response: AxiosResponse<any> = await axios.post(`${_baseUrl}/saveconfig`, config);
    return response.data as ApiResult;
}

export const postMsg = async (msg: string, type: 0 | 1 = 0) => {
    const response: AxiosResponse<any> = await axios.post(`${_baseUrl}/postmsg?type=` + type, msg);
    return response.data as ApiResult;
}

export const getCache = async () => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/getCache`);
    return response.data as ApiResult;
}

export const saveImg = async (id: number) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/saveimg/${id}`);
    return response.data as ApiResult;
}

export const delImg = async (id: number) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/delimg/${id}`);
    return response.data as ApiResult;
}

export const getFun = async (): Promise<ApiResult> => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/getfun`);
    return response.data as ApiResult;
};

export const startPlugin = async (name: string): Promise<ApiResult> => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/startplugin?name=${name}`);
    return response.data as ApiResult;
};

export const stopPlugin = async (name: string): Promise<ApiResult> => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/stopplugin?name=${name}`);
    return response.data as ApiResult;
};

export const delPlugin = async (name: string): Promise<ApiResult> => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/delplugin?name=${name}`);
    return response.data as ApiResult;
};

export const startAliYunPan = async () => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/startaliyunpan`);
    return response.data as ApiResult;
}

export const upload = async (path: string) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/upload?path=${path}`);
    return response.data as ApiResult;
}

export const searchIdol = async (group: string, name: string) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/searchidol?group=${group}&name=${name}`);
    return response.data as ApiResult;
}

export const saveWbByid = async (blogId: string) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/savewbbyid?blogId=${blogId}`);
    return response.data as ApiResult;
}

export const sendSmsCode = async (mobile: string, area = "86") => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/sendsmscode?mobile=${mobile}&area=${area}`);
    return response.data as ApiResult;
}

export const pocketLogin = async (mobile: string, code: string) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/pocketlogin?mobile=${mobile}&code=${code}`);
    return response.data as ApiResult;
}

export const kdUserInfo = async (token: string) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/kduserinfo?token=${token}`);
    return response.data as ApiResult;
}

export const openWindow = async (url: string) => {
    const response: AxiosResponse<any> = await axios.get(`${_baseUrl}/openwindow?url=${url}`);
    return response.data as ApiResult;
}