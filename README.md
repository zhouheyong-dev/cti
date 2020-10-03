# Freeswitch Esl

### 目录结构

Controllers-------------API 控制器目录

Dao---------------------数据库操作层

Entities----------------数据库实体文件

Esl---------------------Freeswitch Event Socket处理

Hubs-------------------- Websocket 库处理

Statics----------------- 资源文件



### 自定义通道变量

1. cid 企业ID
2. XCallID 话单ID
3. XTransferTo 被转接号码
3. XTransferFrom 转接号码来源
4. XTransferType 转接类型
5. XCallerNumber 主叫号码
6. XCalleeNumber 被叫号码
7. XIP IP话机呼叫类型
8. XCallType 呼叫类型 0呼出 1呼入
9. XIsCaller 是否主叫 1 是 0 否

