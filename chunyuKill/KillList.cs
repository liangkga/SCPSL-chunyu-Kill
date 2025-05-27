using System;

namespace ServerKillPlugin
{
	/// <summary>
	/// 击杀信息数据类，存储玩家的击杀记录和相关配置喵~
	/// </summary>
	public class KillList
	{
		/// <summary>
		/// 玩家Steam64位ID，用于唯一标识玩家喵~
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// 击杀音效文件路径，播放击杀时的音效喵~
		/// </summary>
		public string MusicPath { get; set; }

		/// <summary>
		/// 击杀广播消息模板，支持变量替换喵~
		/// </summary>
		public string Broadcast { get; set; }

		/// <summary>
		/// 是否仅击杀者自己可见广播消息喵~
		/// </summary>
		public bool OnlyMe { get; set; }

		/// <summary>
		/// 该玩家击杀SCP的总数统计喵~
		/// </summary>
		public int KillSCPs { get; set; }

		/// <summary>
		/// 该玩家击杀人类的总数统计喵~
		/// </summary>
		public int KillHuman { get; set; }

		/// <summary>
		/// 自定义昵称标签，用于个性化显示喵~
		/// </summary>
		public string CustomTag { get; set; }
	}
}
