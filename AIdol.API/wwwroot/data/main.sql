/*
 Navicat Premium Data Transfer

 Source Server         : bot
 Source Server Type    : SQLite
 Source Server Version : 3035005
 Source Schema         : main

 Target Server Type    : SQLite
 Target Server Version : 3035005
 File Encoding         : 65001

 Date: 26/02/2024 10:37:12
*/

-- ----------------------------
-- Table structure for PoketMessage
-- ----------------------------
DROP TABLE IF EXISTS "PoketMessage";
CREATE TABLE "PoketMessage" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Msg" TEXT NOT NULL,
  "SenderId" integer NOT NULL,
  "ChannelId" text NOT NULL,
  "ChannelName" TEXT NOT NULL,
  "ChannelRole" TEXT NOT NULL,
  "Time" DATE NOT NULL,
  "Type" TEXT NOT NULL,
  "FullInfo" TEXT NOT NULL
);

-- ----------------------------
-- Records of PoketMessage
-- ----------------------------

-- ----------------------------
-- Table structure for QQMessage
-- ----------------------------
DROP TABLE IF EXISTS "QQMessage";
CREATE TABLE "QQMessage" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "SenderId" INTEGER NOT NULL,
  "RecieverId" INTEGER NOT NULL,
  "SenderName" TEXT NOT NULL,
  "RecieverName" TEXT NOT NULL,
  "Content" TEXT NOT NULL,
  "Time" DATE NOT NULL,
  "Url" TEXT
);

-- ----------------------------
-- Records of QQMessage
-- ----------------------------

-- ----------------------------
-- Table structure for SysCache
-- ----------------------------
DROP TABLE IF EXISTS "SysCache";
CREATE TABLE "SysCache" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Content" TEXT NOT NULL,
  "Type" integer NOT NULL,
  "CreateDate" DATE NOT NULL
);

-- ----------------------------
-- Records of SysCache
-- ----------------------------

-- ----------------------------
-- Table structure for SysConfig
-- ----------------------------
DROP TABLE IF EXISTS "SysConfig";
CREATE TABLE "SysConfig" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Pid" INTEGER NOT NULL,
  "Key" TEXT NOT NULL,
  "Name" TEXT NOT NULL,
  "Value" TEXT,
  "Desc" TEXT,
  "DateType" text NOT NULL DEFAULT string
);

-- ----------------------------
-- Records of SysConfig
-- ----------------------------
INSERT INTO "SysConfig" VALUES (1, 0, 'Shamrock', 'OpenShamrock配置', '', '', 'class');
INSERT INTO "SysConfig" VALUES (2, 1, 'Host', '地址', '127.0.0.1', 'OpenShamrock服务的地址，不带协议(如http)', 'string');
INSERT INTO "SysConfig" VALUES (3, 1, 'WebsocktPort', 'Websocket端口', '7001', NULL, 'int');
INSERT INTO "SysConfig" VALUES (4, 1, 'HttpPort', 'Http端口', '7002', NULL, 'int');
INSERT INTO "SysConfig" VALUES (5, 1, 'Token', 'Token', '', '发送数据身份验证标识', 'string');
INSERT INTO "SysConfig" VALUES (6, 1, 'Use', '使用shamrock', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (7, 0, 'EnableModule', '启用项', NULL, NULL, 'list&class');
INSERT INTO "SysConfig" VALUES (8, 7, 'QQ', 'QQ', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (9, 7, 'WB', '微博', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (10, 7, 'BZ', 'B站', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (11, 7, 'KD', '口袋48', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (12, 7, 'XHS', '小红书', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (13, 7, 'DY', '抖音', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (14, 7, 'BD', '百度', 'false', NULL, 'class&bool');
INSERT INTO "SysConfig" VALUES (15, 8, 'Admin', '超管', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (16, 8, 'Permission', '管理员', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (17, 8, 'Group', '群', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (18, 8, 'Save', '保存群消息', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (19, 8, 'Debug', '开启程序错误通知', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (20, 8, 'Notice', '开启消息通知', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (21, 8, 'FuncAdmin', '仅管理员可用', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (22, 9, 'UserAll', '微博用户', NULL, '转发动态，保存图片', 'list');
INSERT INTO "SysConfig" VALUES (23, 9, 'UserPart', '微博用户', NULL, '仅用于保存图片', 'list');
INSERT INTO "SysConfig" VALUES (24, 9, 'TimeSpan', '监听间隔', '3', '单位分钟', 'int');
INSERT INTO "SysConfig" VALUES (25, 9, 'Group', 'qq群', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (26, 9, 'QQ', '好友qq', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (27, 9, 'ForwardGroup', '转发至群', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (28, 9, 'ForwardQQ', '转发好友', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (29, 9, 'ChiGuaUser', '吃瓜用户', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (30, 9, 'Keyword', '关键词过滤', NULL, '吃瓜微博关键词过滤', 'list');
INSERT INTO "SysConfig" VALUES (31, 10, 'User', 'B站用户', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (32, 10, 'TimeSpan', '监听间隔', '3', '单位分钟', 'int');
INSERT INTO "SysConfig" VALUES (33, 10, 'Group', 'qq群', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (34, 10, 'QQ', '好友QQ', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (35, 10, 'ForwardGroup', '转发至群', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (36, 10, 'ForwardQQ', '转发好友', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (37, 11, 'IdolName', '小偶像名', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (38, 11, 'Account', '账号(account)', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (39, 11, 'Token', '标识(token)', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (40, 11, 'ServerId', '服务器ID(serverId)', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (41, 11, 'AppKey', 'AppKey', 'NjMyZmVmZjFmNGM4Mzg1NDFhYjc1MTk1ZDFjZWIzZmE=', NULL, 'string');
INSERT INTO "SysConfig" VALUES (42, 11, 'ImgDomain', '图片域名', 'https://source3.48.cn', NULL, 'string');
INSERT INTO "SysConfig" VALUES (43, 11, 'VideoDomain', '视频域名', 'https://mp4.48.cn', NULL, 'string');
INSERT INTO "SysConfig" VALUES (44, 11, 'LiveRoomId', '直播间(liveId)', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (45, 11, 'MsgTypeAll', '所有消息类型', '[{
    "name":"文本消息",
    "value":"text"
},
{
    "name":"图片消息",
    "value":"image"
},
{
    "name":"表情消息",
    "value":"EXPRESSIMAGE"
},
{
    "name":"视频消息",
    "value":"video,VIDEO"
},
{
    "name":"音频消息",
    "value":"audio,AUDIO"
},
{
    "name":"回复消息",
    "value":"REPLY"
},
{
    "name":"礼物回复消息",
    "value":"GIFTREPLY"
},
{
    "name":"计分消息",
    "value":"fen"
},
{
    "name":"开播",
    "value":"LIVEPUSH"
},
{
    "name":"房间电台",
    "value":"TEAM_VOICE"
},
{
    "name":"开播/房间电台艾特全体成员",
    "value":"AtAll"
},
{
    "name":"文字翻牌",
    "value":"FLIPCARD"
},
{
    "name":"语音翻牌",
    "value":"FLIPCARD_AUDIO"
},
{
    "name":"视频翻牌",
    "value":"FLIPCARD_VIDEO"
},
{
    "name":"礼物消息",
    "value":"PRESENT_NORMAL"
}]', NULL, 'list');
INSERT INTO "SysConfig" VALUES (46, 11, 'MsgType', '消息类型', NULL, '接收/转发消息类型', 'list');
INSERT INTO "SysConfig" VALUES (47, 12, 'User', '小红书用户', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (48, 12, 'TimeSpan', '监听间隔', '3', '单位分钟', 'int');
INSERT INTO "SysConfig" VALUES (49, 12, 'Group', 'qq群', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (50, 12, 'ForwardGroup', '转发至群', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (51, 12, 'ForwardQQ', '转发好友', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (52, 12, 'QQ', '好友qq', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (53, 13, 'User', '抖音用户', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (54, 13, 'TimeSpan', '监听间隔', '3', '单位分钟', 'int');
INSERT INTO "SysConfig" VALUES (55, 13, 'Group', 'qq群', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (56, 13, 'QQ', '好友qq', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (57, 13, 'ForwardGroup', '转发至群', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (58, 13, 'ForwardQQ', '转发好友', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (59, 14, 'AppKey', 'AppKey', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (60, 14, 'AppSeret', 'AppSeret', NULL, NULL, 'string');
INSERT INTO "SysConfig" VALUES (61, 14, 'Similarity', '人脸相似度', '80', '*直接保存(非双胞胎建议80，双胞胎建议70)', 'int');
INSERT INTO "SysConfig" VALUES (62, 14, 'SaveAliyunDisk', '保存到阿里云盘', 'false', NULL, 'string');
INSERT INTO "SysConfig" VALUES (63, 14, 'Audit', '审核相似度', '50', NULL, 'int');
INSERT INTO "SysConfig" VALUES (64, 14, 'AlbumName', '相册名', NULL, '如果不存在，自动创建', 'string');
INSERT INTO "SysConfig" VALUES (65, 14, 'FaceVerify', '开启人脸验证', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (66, 14, 'ImageList', '基础人脸', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (67, 9, 'WBChiGuaForwardGroup', '吃瓜转发至群', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (68, 9, 'WBChiGuaForwardQQ', '吃瓜转发QQ', 'false', NULL, 'bool');
INSERT INTO "SysConfig" VALUES (69, 9, 'WBChiGuaQQ', '吃瓜转发qq', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (70, 9, 'WBChiGuaGroup', '吃瓜转发群', NULL, NULL, 'list');
INSERT INTO "SysConfig" VALUES (71, 11, 'SaveMsg', '保存消息', '1', '0-不保存，1-小偶像消息，2-全部消息', 'string');

-- ----------------------------
-- Table structure for SysIdol
-- ----------------------------
DROP TABLE IF EXISTS "SysIdol";
CREATE TABLE "SysIdol" (
  "Id" TEXT NOT NULL,
  "Name" TEXT NOT NULL,
  "RoomId" text NOT NULL,
  "Account" TEXT NOT NULL,
  "ServerId" text,
  "TeamId" text,
  "LiveId" text,
  "Team" TEXT,
  "GroupName" TEXT,
  "PeriodName" TEXT,
  "PinYin" TEXT,
  "ChannelId" text,
  PRIMARY KEY ("Id")
);

-- ----------------------------
-- Records of SysIdol
-- ----------------------------

-- ----------------------------
-- Table structure for SysLog
-- ----------------------------
DROP TABLE IF EXISTS "SysLog";
CREATE TABLE "SysLog" (
  "Id" integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Content" TEXT,
  "Time" DATE
);

-- ----------------------------
-- Records of SysLog
-- ----------------------------

-- ----------------------------
-- Table structure for SysUser
-- ----------------------------
DROP TABLE IF EXISTS "SysUser";
CREATE TABLE "SysUser" (
  "Id" text NOT NULL,
  "Role" integer NOT NULL,
  "Nickname" TEXT NOT NULL,
  "Vip" real NOT NULL,
  "Avatar" TEXT NOT NULL,
  "Level" integer NOT NULL,
  "PfUrl" TEXT NOT NULL,
  "TeamLogo" TEXT NOT NULL,
  "LastActive" date NOT NULL,
  PRIMARY KEY ("Id")
);

-- ----------------------------
-- Records of SysUser
-- ----------------------------