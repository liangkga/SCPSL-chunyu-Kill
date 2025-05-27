# chunyuKill - SCP:SL 击杀统计插件

一个用于 SCP: Secret Laboratory 服务器的击杀统计插件，基于 Exiled 框架开发。

## 功能特性

- 📊 **击杀统计追踪** - 实时记录玩家击杀人类和SCP的数量
- 📢 **广播提示** - 击杀时可显示自定义广播消息
- 💾 **数据持久化** - 击杀数据自动保存到josn文件
- 🎯 **个性化配置** - 支持为不同玩家设置不同的击杀提示

## 安装要求

- SCP: Secret Laboratory 服务器
- Exiled 框架 (版本 9.6.0 或更高)
- .NET Framework 4.8 或更高版本

## 安装方法

1. 下载最新版本的 `chunyuKill.dll` 文件
2. 将文件放置到服务器的 `Exiled/Plugins` 目录中
3. 重启服务器或重新加载插件
4. 插件将自动生成配置文件

## 配置说明

### 主配置 (Config.cs)

```yaml
chunyu_kill:
  # 是否启用插件
  is_enabled: true
  
  # 是否启用调试模式
  debug: false
  
  # 是否在回合结束时启用友伤
  is_enable_round_ended_f_f: false
  
  # 玩家击杀配置列表
  infos:
  - user_id: "玩家Steam ID"
    music_path: "音效文件路径"
    broadcast: "击杀时显示的广播消息"
    only_me: true
    kill_sc_ps: 0
    kill_human: 0
    custom_tag: "自定义标签"
```

### 配置项说明

- `is_enabled`: 控制插件是否启用
- `debug`: 启用调试模式，输出详细日志
- `is_enable_m_v_p`: 是否在回合结束时显示MVP
- `is_enable_round_ended_f_f`: 回合结束时是否启用友伤
- `infos`: 玩家个性化配置列表
  - `user_id`: 玩家的Steam ID
  - `music_path`: 击杀时播放的音效文件路径
  - `broadcast`: 击杀时显示的广播消息
  - `only_me`: 是否只对该玩家显示消息
  - `kill_sc_ps`: SCP击杀数量
  - `kill_human`: 人类击杀数量
  - `custom_tag`: 自定义标签

## 数据存储

插件会在以下位置保存击杀数据：
- 路径: `Exiled/Configs/Ports/{端口号}/chunyuKill_KillData.json`
- 格式: JSON格式，包含所有玩家的击杀统计信息

## 命令说明

目前插件主要通过配置文件进行管理，暂无游戏内命令。

## 事件处理

插件监听以下游戏事件：
- 玩家死亡事件 - 统计击杀数据
- 玩家验证事件 - 加载玩家数据
- 玩家离开事件 - 保存玩家数据
- 回合结束事件 - 显示MVP信息

## 开发信息

- **作者**: chunyu
- **版本**: 1.0.0.beta
- **框架**: Exiled 9.6.0+
- **语言**: C#
- **许可证**: MIT

## 更新日志

### v1.0.0
- 初始版本发布
- 基础击杀统计功能
- MVP系统实现
- 数据持久化支持
- 个性化配置系统

## 技术支持

如果您在使用过程中遇到问题，请：
1. 检查服务器日志中的错误信息
2. 确认Exiled框架版本兼容性
3. 验证配置文件格式是否正确
4. 联系开发者获取技术支持

## 贡献

欢迎提交问题报告和功能建议！

---

**注意**: 本插件仅适用于 SCP: Secret Laboratory 服务器，请确保您有合法的服务器运营权限。
