using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace ServerKillPlugin
{
	/// <summary>
	/// 插件主类，继承自Exiled插件基类，负责插件的生命周期管理喵~
	/// </summary>
	public class Plugin : Exiled.API.Features.Plugin<Config>
	{
		/// <summary>
		/// 插件作者信息喵~
		/// </summary>
		public override string Author { get; } = "椿雨";
		
		/// <summary>
		/// 插件名称喵~
		/// </summary>
		public override string Name { get; } = "铭刻";
		
		/// <summary>
		/// 插件版本号喵~
		/// </summary>
		public override Version Version { get; } = new Version(1, 0, 0);
		
		/// <summary>
		/// 所需的Exiled最低版本喵~
		/// </summary>
		public override Version RequiredExiledVersion { get; } = new Version(9, 6, 0);

		/// <summary>
		/// 击杀追踪器实例，负责处理击杀相关的事件和逻辑喵~
		/// </summary>
		private MainPlugin _killTracker;

		/// <summary>
		/// 插件启用时的初始化方法，注册事件处理器喵~
		/// </summary>
		public override void OnEnabled()
		{
			base.OnEnabled();
			// 创建击杀追踪器实例喵~
			_killTracker = new MainPlugin();
			// 启用击杀追踪器，注册相关事件喵~
			_killTracker.OnEnabled();
			// 输出插件启动成功日志喵~
			Log.Info($"{Name} 插件已开启版本: {Version}，作者: {Author} 喵~");
		}

		/// <summary>
		/// 插件禁用时的清理方法，注销事件处理器喵~
		/// </summary>
		public override void OnDisabled()
		{
			base.OnDisabled();
			// 检查击杀追踪器是否存在喵~
			if (_killTracker != null)
			{
				// 禁用击杀追踪器，注销相关事件喵~
				_killTracker.OnDisabled();
				// 清理击杀追踪器实例，释放内存喵~
				_killTracker = null;
			}
		}







	}
}
