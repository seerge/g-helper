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