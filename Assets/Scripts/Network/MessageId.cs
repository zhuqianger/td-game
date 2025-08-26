namespace Network
{
    public enum MessageId
    {
        // 登录相关
        REQ_LOGIN = 1,
        RESP_LOGIN = 2,
        
        // 玩家相关
        REQ_CREATE_PLAYER = 10,
        RESP_CREATE_PLAYER = 11,
        
        REQ_GET_PLAYER_INFO = 12,
        RESP_GET_PLAYER_INFO = 13,
        
        REQ_UPDATE_PLAYER = 14,
        RESP_UPDATE_PLAYER = 15,
        
        REQ_DELETE_PLAYER = 16,
        RESP_DELETE_PLAYER = 17,
        
        REQ_GET_ALL_PLAYERS = 18,
        RESP_GET_ALL_PLAYERS = 19,
        
        // 干员相关
        REQ_GET_PLAYER_OPERATORS = 34,
        RESP_GET_PLAYER_OPERATORS = 35,
        
        REQ_LEVEL_UP_OPERATOR = 38,
        RESP_LEVEL_UP_OPERATOR = 39,
        
        REQ_ELITE_OPERATOR = 40,
        RESP_ELITE_OPERATOR = 41,
        
        REQ_UPGRADE_SKILL = 42,
        RESP_UPGRADE_SKILL = 43,
        
        REQ_MASTER_SKILL = 44,
        RESP_MASTER_SKILL = 45,
        
        REQ_UPDATE_OPERATOR_HP = 46,
        RESP_UPDATE_OPERATOR_HP = 47,
        
        // 背包相关
        REQ_GET_BACKPACK = 60,
        RESP_GET_BACKPACK = 61,
        
        REQ_GET_BACKPACK_BY_TYPE = 62,
        RESP_GET_BACKPACK_BY_TYPE = 63,
        
        REQ_USE_ITEM = 66,
        RESP_USE_ITEM = 67,
        
        // 关卡相关
        REQ_GET_PLAYER_STAGES = 82,
        RESP_GET_PLAYER_STAGES = 83,
        
        REQ_SAVE_STAGE_RECORD = 86,
        RESP_SAVE_STAGE_RECORD = 87,
        
        // 错误
        ERROR_MSG = -1
    }
}