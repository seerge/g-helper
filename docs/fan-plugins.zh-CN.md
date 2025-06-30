# 风扇插件系统

## 功能简介

风扇插件系统允许用户通过编写自定义的 Lua 脚本来动态控制风扇转速。这取代了传统基于温度点的静态风扇曲线，为您提供了更灵活、更强大的控制能力，可以根据多种传感器数据实时调整风扇行为。

## 如何使用

1.  **启用插件**:
    *   导航到 G-Helper 的“风扇 + 电源”设置页面。
    *   找到并勾选“启用风扇插件”复选框。
    *   启用后，下方的风扇曲线设置将被禁用，取而代之的是一个插件选择下拉框。

2.  **选择插件**:
    *   点击下拉框，您会看到所有可用的风扇插件。
    *   选择一个插件后，系统会立即加载并开始使用其逻辑来控制风扇。

3.  **添加自定义插件**:
    *   要添加您自己的插件，只需将您的 `.lua` 文件放入 G-Helper 安装目录下的 `app/Plugins/fans/` 文件夹中。
    *   重启 G-Helper 后，您的插件将自动出现在下拉列表中。

## 如何创建插件

创建自己的风扇插件非常简单，您只需要了解其基本的文件结构和核心 API。

### 文件结构

所有风扇插件都必须是标准的 Lua 脚本文件（扩展名为 `.lua`），并存放于 `app/Plugins/fans/` 目录下。

### 核心 API

每个插件的核心都是一个名为 `update` 的全局函数。G-Helper 会定期调用这个函数，传递最新的传感器数据，并期望返回一个包含风扇转速指令的表格。

#### `update` 函数

```lua
function update(sensors, dt)
    -- 你的逻辑代码在这里
    
    return {
        cpu_fan = 50, -- CPU 风扇转速 (0-100)
        gpu_fan = 50, -- GPU 风扇转速 (0-100)
        mid_fan = 50  -- 中间风扇转速 (0-100)，如果存在
    }
end
```

#### 输入参数

`update` 函数接收两个参数：

1.  **`sensors` (table)**: 这个表格包含了从系统硬件中读取的各种实时数据。您可以使用这些数据来构建您的控制逻辑。
    *   `cpu_temp`: CPU 温度 (摄氏度)
    *   `gpu_temp`: GPU 温度 (摄氏度)
    *   以及其他可用的传感器数据...
    *   *注意: 访问一个不存在的传感器键值会返回 `nil`。建议在使用前进行检查。*

2.  **`dt` (number)**: 距离上次调用 `update` 的时间差，单位是秒。这对于时间相关的计算（例如 PID 控制器中的计算）至关重要。

#### 输出: 风扇转速表格

`update` 函数**必须**返回一个表格，其中包含风扇转速的键和对应的值（0 到 100 之间的整数）。

*   `cpu_fan`: CPU 风扇的目标转速。
*   `gpu_fan`: GPU 风扇的目标转速。
*   `mid_fan`: 中间风扇（如果您的设备有）的目标转速。

G-Helper 将使用这些值来设置相应的风扇转速。

## 示例插件: `default.lua` (PID 控制器)

以下是 G-Helper 附带的默认插件。它实现了一个 PID (比例-积分-微分) 控制器，用于动态调整风扇速度，从而将 CPU 和 GPU 的温度稳定在目标值附近。这是一个很好的高级起点。

### 完整代码

```lua
--- G-Helper PID 温度控制器 (默认)
-- @author shadow3aaa@github.com
-- @version 2.0
--[[
  该插件为 CPU 和 GPU 实现了一个 PID (比例-积分-微分) 控制器。
  其目标是动态调整各自的风扇速度，以将硬件温度稳定在设定的目标值附近。

  这是 G-Helper 的默认风扇插件实现。
]]

-- ===================================================================
-- PID 控制器配置 (用户可调节)
-- ===================================================================

local cpu_pid = {
  -- 目标温度 (摄氏度)
  target_temperature = 93,
  -- P (比例) - Kp: 响应当前的误差大小。
  Kp = 12.0, -- CPU 温度变化通常更剧烈，需要更快的响应
  -- I (积分) - Ki: 累积过去的误差。
  Ki = 0.8,
  -- D (微分) - Kd: 响应误差的变化率。
  Kd = 1.2,
  -- 状态变量 (请勿修改)
  integral = 0,
  previous_error = 0
}

local gpu_pid = {
  -- 目标温度 (摄氏度)
  target_temperature = 80,
  -- P (比例) - Kp:
  Kp = 10.0,
  -- I (积分) - Ki:
  Ki = 0.5,
  -- D (微分) - Kd:
  Kd = 1.0,
  -- 状态变量 (请勿修改)
  integral = 0,
  previous_error = 0
}

-- 积分饱和限制 (全局)
local integral_max = 100
local integral_min = -100

--[[
  reset 函数用于初始化或重置所有 PID 控制器的状态。
  G-Helper 会在插件被激活时调用此函数。
]]
function reset()
  cpu_pid.integral = 0
  cpu_pid.previous_error = 0
  gpu_pid.integral = 0
  gpu_pid.previous_error = 0
end

--[[
  calculate_fan_speed 是一个通用的 PID 计算函数。
  @param pid_controller (table): 包含特定控制器参数和状态的表 (例如 cpu_pid)
  @param current_temperature (number): 来自传感器的当前温度
  @param dt (number): 时间增量
  @return (number): 计算出的风扇转速 (0-100)
]]
function calculate_fan_speed(pid_controller, current_temperature, dt)
  -- 1. 计算误差
  local error = current_temperature - pid_controller.target_temperature

  -- 2. 计算 PID 各项
  -- 积分项
  pid_controller.integral = pid_controller.integral + error * dt
  pid_controller.integral = math.max(integral_min, math.min(integral_max, pid_controller.integral))

  -- 微分项
  local derivative = (error - pid_controller.previous_error) / dt
  
  -- 3. 计算最终的 PID 输出
  local pid_output = (pid_controller.Kp * error) + (pid_controller.Ki * pid_controller.integral) + (pid_controller.Kd * derivative)

  -- 4. 更新状态
  pid_controller.previous_error = error

  -- 5. 将 PID 输出转换为风扇转速
  local base_fan_speed = 30
  local fan_speed = base_fan_speed + pid_output
  
  -- 6. 将风扇转速限制在安全范围内 (30% - 100%)
  return math.max(base_fan_speed, math.min(100, fan_speed))
end

--[[
  update 函数是此插件的入口点。
  @param sensors (table): 包含当前传感器数据的表 (sensors.cpu_temp, sensors.gpu_temp)
  @param dt (number): 自上次调用以来的时间增量 (秒)
  @return (table): 包含风扇转速百分比的表 (0-100)
]]
function update(sensors, dt)
  local fan_speeds = {}

  if dt == nil or dt == 0 then
    return {} -- 无效的 dt，不进行调整
  end

  -- 计算 CPU 风扇转速
  if sensors.cpu_temp then
    fan_speeds.cpu_fan = calculate_fan_speed(cpu_pid, sensors.cpu_temp, dt)
  end

  -- 计算 GPU 风扇转速
  if sensors.gpu_temp then
    fan_speeds.gpu_fan = calculate_fan_speed(gpu_pid, sensors.gpu_temp, dt)
  end
  
  return fan_speeds
end

-- 首次加载脚本时重置状态
reset()
```

### 逻辑解释

1.  **配置 (`cpu_pid`, `gpu_pid`)**:
    *   脚本定义了 `cpu_pid` 和 `gpu_pid` 两个表，用于分别保存 CPU 和 GPU 控制器的配置。
    *   `target_temperature`: 控制器试图维持的目标温度（摄氏度）。
    *   `Kp` (比例): 提供对当前温度误差的即时、成比例的响应。值越高，响应越快越强。
    *   `Ki` (积分): 随时间累积误差。这有助于消除长期的稳态误差，确保温度最终达到目标。
    *   `Kd` (微分): 通过对温度变化率的反应来预测未来的误差。这有助于抑制超调和振荡。

2.  **状态管理 (`reset` 函数)**:
    *   G-Helper 在每次激活插件时都会调用 `reset` 函数。
    *   它将 `integral` 和 `previous_error` 状态变量初始化为零。这对于确保 PID 计算有一个干净、可预测的开始至关重要，避免受到先前状态的影响。

3.  **PID 计算 (`calculate_fan_speed` 函数)**:
    *   这是控制器的核心。它是一个通用函数，可以为任何给定的 PID 控制器配置计算风扇速度。
    *   **步骤 1-2**: 计算 `error` (当前温度与目标温度的差异)，然后计算 P、I、D 三个项。积分项被限制在一定范围内，以防止其增长过大（即“积分饱和”）。
    *   **步骤 3**: 将 P、I、D 三项相加得到最终的 `pid_output`。
    *   **步骤 4-6**: 更新 `previous_error` 状态以供下次计算使用，将原始 PID 输出转换为风扇转速百分比（将其添加到基础速度上），最后将结果限制在一个安全且实用的范围内 (30-100%)。

4.  **主入口点 (`update` 函数)**:
    *   G-Helper 会定期调用此函数，并传入最新的 `sensors` 数据和 `dt` (时间差)。
    *   它会检查 `dt` 是否有效，以防止出现除以零的错误。
    *   然后，如果相应的温度数据可用，它会为 CPU 和 GPU 风扇调用通用的 `calculate_fan_speed` 函数。
    *   最后，它返回一个包含计算出的风扇速度的表，G-Helper 随后会应用这些速度。