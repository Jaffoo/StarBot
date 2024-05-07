import axios from 'axios';
import type { AxiosResponse } from 'axios';
import type { Config, ApiResult } from '@/class/model';

const _port = sessionStorage.getItem("HttpPort") || 5266;
//const _baseUrl = `http://localhost:${_port}/api/v1`;
const _baseUrl = `http://154.201.76.32:${_port}/api/v1`;

export const ApiPrefix = _baseUrl;

export const startBot = async (botReady = true) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/startbot?botReady=${botReady}`);
    return response.data;
}

export const getConfig = async () => {
    const response= await axios.get<ApiResult>(`${_baseUrl}/getconfig`);
    return response.data
}

export const saveConfig = async (config: Config) => {
    const response= await axios.post<ApiResult>(`${_baseUrl}/saveconfig`, config);
    return response.data;
}

export const postMsg = async (msg: object, type: 0 | 1 = 0) => {
    const response = await axios.post<ApiResult>(`${_baseUrl}/postmsg?type=` + type, msg);
    return response.data;
}

export const getCache = async (page: any) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/getCache?pageIndex=${page.pageIndex}&pageSize=${page.pageSize}`);
    return response.data;
}

export const saveImg = async (id: number) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/saveimg/${id}`);
    return response.data;
}

export const delImg = async (id: number) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/delimg/${id}`);
    return response.data;
}

export const getFun = async () => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/getfun`);
    return response.data;
};

export const startPlugin = async (name: string,version:string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/startplugin?name=${name}&version=${version}`);
    return response.data;
};

export const stopPlugin = async (name: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/stopplugin?name=${name}`);
    return response.data;
};

export const delPlugin = async (name: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/delplugin?name=${name}`);
    return response.data;
};

export const startAliYunPan = async () => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/startaliyunpan`);
    return response.data;
}

export const upload = async (path: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/upload?path=${path}`);
    return response.data;
}

export const uploaddll = async (path: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/uploaddll?path=${path}`);
    return response.data;
}

export const searchIdol = async (group: string, name: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/searchidol?group=${group}&name=${name}`);
    return response.data;
}

export const saveWbByid = async (blogId: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/savewbbyid?blogId=${blogId}`);
    return response.data;
}

export const sendSmsCode = async (mobile: string, area = "86") => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/sendsmscode?mobile=${mobile}&area=${area}`);
    return response.data;
}

export const pocketLogin = async (mobile: string, code: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/pocketlogin?mobile=${mobile}&code=${code}`);
    return response.data;
}

export const kdUserInfo = async (data: object) => {
    const response = await axios.post(`${_baseUrl}/kduserinfo`, data);
    return response.data;
}

export const openWindow = async (url: string) => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/openwindow?url=${url}`);
    return response.data;
}

export const botInfo = async () => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/botinfo`);
    return response.data;
}

export const getLogs = async () => {
    const response = await axios.get<ApiResult>(`${_baseUrl}/getlogs`);
    return response.data;
}