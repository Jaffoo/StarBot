interface Base {
    group?: string,
    forwardGroup: boolean,
    forwardQQ: boolean,
    qq?: string,
}
interface Listing extends Base {
    timeSpan: number,
}
export interface Config {
    BD?: BD,
    QQ?: QQ,
    Shamrock?: Shamrock,
    DY?: DY_XHS_BZ,
    BZ?: DY_XHS_BZ,
    XHS?: DY_XHS_BZ,
    KD?: KD,
    WB?: WB,
    EnableModule: EnableModule
}

export interface BD {
    appKey?: string,
    appSeret?: string,
    similarity?: number,
    saveAliyunDisk: boolean,
    audit?: number,
    albumName?: string,
    faceVerify: boolean,
    imageList?: Array<string>
}
export interface MsgType {
    name: string,
    value: string
}
export interface KD extends Base {
    idolName?: string,
    account?: string,
    token?: string,
    serverId?: string,
    liveRoomId?: string,
    msgTypeAll?: Array<MsgType>,
    msgType?: Array<string>,
    saveMsg?: number,
    saveImg?: boolean
}
export interface WB extends Listing {
    userAll?: string,
    userPart?: string,
    chiGuaUser?: string,
    keyword?: string,
    chiGuaQQ?: string,
    chiGuaForwardQQ: boolean,
    chiGuaGroup?: string,
    chiGuaForwardGroup: boolean
}
export interface DY_XHS_BZ extends Listing {
    user: string,
}
export interface QQ {
    funcAdmin?: Array<string>,
    group?: string,
    save: boolean,
    admin: string,
    notice: boolean,
    permission: string,
    debug: boolean,
}
export interface Shamrock {
    use: boolean,
    host: string,
    websocktPort: number,
    httpPort: number,
    token?: string,
}
export interface EnableModule {
    shamrock: boolean,
    qq: boolean,
    wb: boolean,
    kd: boolean,
    dy: boolean,
    bd: boolean,
    xhs: boolean,
    bz: boolean,
}
export interface ApiResult {
    code: number,
    msg?: string,
    data?: any,
    success: boolean,
    count: number
}