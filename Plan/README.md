# 龍與地下城 
## 工項安排
### Manager
- [ ] 地圖製作
    - [x] 地圖編輯器 prototype (榮)
    - [ ] 地圖編輯器 互動分析 (杰)
    - [ ] 地圖編輯器 資料結構 (榮)
    - [ ] 地圖編輯器 unit test (榮)
    - [ ] 地圖編輯器 final code (榮)
    - [ ] 地形 prefab (杰)
    - [ ] 關卡一地形 (杰)
    - [ ] 關卡二地形 (杰)
    - [ ] 關卡三地形 (杰)
- [ ] 戰鬥流程
    - [ ] 基本流程說明 (杰)
    - [ ] 玩家行為分析 (杰)
    - [ ] 資料結構 (榮)
    - [ ] 流程 (榮)
    - [ ] 流程 unit test (榮)
- [ ] 文本樹
    - [ ] 文本編輯器 prototype (榮)
        - [ ] 條件觸發 (榮)
    - [ ] 文本編輯器 unit test (榮)
    - [ ] 文本編輯器 final code (榮)
## 專案分層
### Business Layer
dnd 核心玩法
### User Case
使用者透過我們這套系統會做的一切行為
* Idle
* Move
* Interactive
### User Interact
使用者交互
1. 展示層
    * CharacterD
    * TileD
1. 輸入層
    * MainProcess