# Fan Plugin System

## Feature Introduction

The fan plugin system allows users to dynamically control fan speeds by writing custom Lua scripts. This replaces the traditional static fan curve based on temperature points, providing you with more flexible and powerful control to adjust fan behavior in real-time based on various sensor data.

## How to Use

1.  **Enable Plugins**:
    *   Navigate to the "Fans + Power" settings page in G-Helper.
    *   Find and check the "Enable Fan Plugins" checkbox.
    *   Once enabled, the fan curve settings below will be disabled and replaced by a plugin selection dropdown.

2.  **Select a Plugin**:
    *   Click the dropdown to see all available fan plugins.
    *   After selecting a plugin, the system will immediately load it and start using its logic to control the fans.

3.  **Add Custom Plugins**:
    *   To add your own plugin, simply place your `.lua` file into the `app/Plugins/fans/` folder in the G-Helper installation directory.
    *   After restarting G-Helper, your plugin will automatically appear in the dropdown list.

## How to Create a Plugin

Creating your own fan plugin is very simple. You just need to understand its basic file structure and core API.

### File Structure

All fan plugins must be standard Lua script files (with a `.lua` extension) and stored in the `app/Plugins/fans/` directory.

### Core API

The core of every plugin is a global function named `update`. G-Helper calls this function periodically, passing the latest sensor data, and expects a table containing fan speed commands in return.

#### `update` Function

```lua
function update(sensors, dt)
    -- Your logic code here
    
    return {
        cpu_fan = 50, -- CPU fan speed (0-100)
        gpu_fan = 50, -- GPU fan speed (0-100)
        mid_fan = 50  -- Middle fan speed (0-100), if present
    }
end
```

#### Input Parameters

The `update` function receives two arguments:

1.  **`sensors` (table)**: This table contains various real-time data read from the system's hardware. You can use this data to build your control logic.
    *   `cpu_temp`: CPU Temperature (Celsius)
    *   `gpu_temp`: GPU Temperature (Celsius)
    *   And other available sensor data...
    *   *Note: Accessing a non-existent sensor key will return `nil`. It's recommended to check before use.*

2.  **`dt` (number)**: The time delta in seconds since the last call to `update`. This is crucial for time-dependent calculations like those in a PID controller.

#### Output: Fan Speed Table

The `update` function **must** return a table containing fan speed keys and their corresponding values (an integer between 0 and 100).

*   `cpu_fan`: The target speed for the CPU fan.
*   `gpu_fan`: The target speed for the GPU fan.
*   `mid_fan`: The target speed for the middle fan (if your device has one).

G-Helper will use these values to set the corresponding fan speeds.

## Example Plugin: `default.lua` (PID Controller)

Below is the default plugin included with G-Helper. It implements a PID (Proportional-Integral-Derivative) controller to dynamically adjust fan speeds and maintain a target temperature for both the CPU and GPU. It serves as an excellent advanced starting point.

### Full Code

```lua
--- G-Helper PID Temperature Controller (Default)
-- @author shadow3aaa@github.com
-- @version 2.0
--[[
  This plugin implements a PID (Proportional-Integral-Derivative) controller
  for both the CPU and GPU. Its goal is to dynamically adjust their respective
  fan speeds to stabilize hardware temperatures around a set target value.

  This is the default fan plugin implementation for G-Helper.
]]

-- ===================================================================
-- PID Controller Configuration (User-Adjustable)
-- ===================================================================

local cpu_pid = {
  -- Target temperature in Celsius
  target_temperature = 93,
  -- P (Proportional) - Kp: Responds to the current error size.
  Kp = 12.0, -- CPU temperature often changes more dramatically, requiring a faster response
  -- I (Integral) - Ki: Accumulates past errors.
  Ki = 0.8,
  -- D (Derivative) - Kd: Responds to the rate of change of the error.
  Kd = 1.2,
  -- State variables (Do not modify)
  integral = 0,
  previous_error = 0
}

local gpu_pid = {
  -- Target temperature in Celsius
  target_temperature = 80,
  -- P (Proportional) - Kp:
  Kp = 10.0,
  -- I (Integral) - Ki:
  Ki = 0.5,
  -- D (Derivative) - Kd:
  Kd = 1.0,
  -- State variables (Do not modify)
  integral = 0,
  previous_error = 0
}

-- Integral saturation limits (Global)
local integral_max = 100
local integral_min = -100

--[[
  The reset function is used to initialize or reset the state of all PID controllers.
  G-Helper calls this function when the plugin is activated.
]]
function reset()
  cpu_pid.integral = 0
  cpu_pid.previous_error = 0
  gpu_pid.integral = 0
  gpu_pid.previous_error = 0
end

--[[
  calculate_fan_speed is a generic PID calculation function.
  @param pid_controller (table): A table containing parameters and state for a specific controller (e.g., cpu_pid)
  @param current_temperature (number): The current temperature from the sensor
  @param dt (number): The time delta
  @return (number): The calculated fan speed (0-100)
]]
function calculate_fan_speed(pid_controller, current_temperature, dt)
  -- 1. Calculate the error
  local error = current_temperature - pid_controller.target_temperature

  -- 2. Calculate the PID terms
  -- Integral term
  pid_controller.integral = pid_controller.integral + error * dt
  pid_controller.integral = math.max(integral_min, math.min(integral_max, pid_controller.integral))

  -- Derivative term
  local derivative = (error - pid_controller.previous_error) / dt
  
  -- 3. Calculate the final PID output
  local pid_output = (pid_controller.Kp * error) + (pid_controller.Ki * pid_controller.integral) + (pid_controller.Kd * derivative)

  -- 4. Update the state
  pid_controller.previous_error = error

  -- 5. Convert PID output to fan speed
  local base_fan_speed = 30
  local fan_speed = base_fan_speed + pid_output
  
  -- 6. Clamp the fan speed to a safe range (30% - 100%)
  return math.max(base_fan_speed, math.min(100, fan_speed))
end

--[[
  The update function is the entry point for this plugin.
  @param sensors (table): A table containing current sensor data (sensors.cpu_temp, sensors.gpu_temp)
  @param dt (number): The time delta in seconds since the last call
  @return (table): A table containing fan speed percentages (0-100)
]]
function update(sensors, dt)
  local fan_speeds = {}

  if dt == nil or dt == 0 then
    return {} -- Invalid dt, do not adjust
  end

  -- Calculate CPU fan speed
  if sensors.cpu_temp then
    fan_speeds.cpu_fan = calculate_fan_speed(cpu_pid, sensors.cpu_temp, dt)
  end

  -- Calculate GPU fan speed
  if sensors.gpu_temp then
    fan_speeds.gpu_fan = calculate_fan_speed(gpu_pid, sensors.gpu_temp, dt)
  end
  
  return fan_speeds
end

-- Reset state on first script load
reset()
```

### Logic Explanation

1.  **Configuration (`cpu_pid`, `gpu_pid`)**:
    *   The script defines two tables, `cpu_pid` and `gpu_pid`, to hold the configuration for each respective controller.
    *   `target_temperature`: The temperature (in Celsius) the controller will try to maintain.
    *   `Kp` (Proportional): Provides an immediate, proportional response to the current temperature error. A higher value means a stronger, faster response.
    *   `Ki` (Integral): Accumulates error over time. This helps eliminate long-term, steady-state errors, ensuring the temperature eventually reaches the target.
    *   `Kd` (Derivative): Predicts future error by reacting to the rate of temperature change. This helps to dampen overshoot and oscillations.

2.  **State Management (`reset` function)**:
    *   The `reset` function is called by G-Helper whenever the plugin is activated.
    *   It initializes the `integral` and `previous_error` state variables to zero. This is crucial for ensuring a clean, predictable start for the PID calculations, preventing influence from previous states.

3.  **PID Calculation (`calculate_fan_speed` function)**:
    *   This is the core of the controller. It's a generic function that can calculate fan speed for any given PID controller configuration.
    *   **Step 1-2**: It calculates the `error` (the difference between current and target temperature) and then computes the three PID terms. The integral term is clamped to prevent it from growing too large ("integral windup").
    *   **Step 3**: The P, I, and D terms are summed to get a final `pid_output`.
    *   **Step 4-6**: It updates the `previous_error` state for the next calculation, converts the raw PID output to a fan speed percentage (adding it to a base speed), and finally clamps the result to a safe and practical range (30-100%).

4.  **Main Entry Point (`update` function)**:
    *   This function is called periodically by G-Helper, which passes in the latest `sensors` data and `dt` (time delta).
    *   It checks for a valid `dt` to prevent division by zero errors.
    *   It then calls the generic `calculate_fan_speed` function for the CPU and GPU fans, provided their temperature data is available.
    *   Finally, it returns a table with the calculated fan speeds, which G-Helper then applies.