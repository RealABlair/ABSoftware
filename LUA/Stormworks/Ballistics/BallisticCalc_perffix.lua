local math_cos, math_sin, math_atan, math_sqrt, math_pi, math_log = math.cos, math.sin, math.atan, math.sqrt, math.pi, math.log

local DATA = {
    [0] = {800, 120, 0.005},
    [1] = {1000, 150, 0.02},
    [2] = {1000, 300, 0.01},
    [3] = {900, 600, 0.005},
    [4] = {800, 1500, 0.002},
    [5] = {700, 2400, 0.001},
    [6] = {600, 2400, 0.0005}
}
local G = 30
local D2R = 0.0174533

local function get_height_error(ang_deg, h, dz, v_tick, g_tick, coeff, drag_inv)
    local pitch_rad = ang_deg * D2R
    local cos_p = math_cos(pitch_rad)
    local sin_p = math_sin(pitch_rad)
    
    if cos_p < 0.0001 then cos_p = 0.0001 end
    
    local ttf = 0
    local arg = 1 - (h * coeff) / (v_tick * cos_p)
    if arg > 0 then
        ttf = -math_log(arg) / coeff
    else
        ttf = h / (v_tick * cos_p)
    end
    
    local p_drag = drag_inv^ttf
    local y_predicted = ((v_tick * sin_p * coeff + g_tick) * (1 - p_drag) - g_tick * coeff * ttf) / (coeff * coeff)
    
    return y_predicted - dz, ttf
end

function onTick()
    local ind = input.getBool(1)
    local sx, sy, sz = input.getNumber(1), input.getNumber(2), input.getNumber(6)
    local tx, ty, tz = input.getNumber(3), input.getNumber(4), input.getNumber(7)
    local cmps = input.getNumber(5)
    local wt = property.getNumber("Weapon Type")
    
    if tx == 0 and ty == 0 then
        output.setNumber(1, 0); output.setNumber(2, 0); output.setNumber(3, -1)
        return
    end

    local weapon = DATA[wt] or DATA[0]
    local v0, coeff = weapon[1], weapon[3]
    local v_tick = v0 / 60
    local g_tick = G / 3600
    local drag_inv = 1 - coeff

    local dx = tx - sx
    local dy = ty - sy
    local dz = tz - sz
    local h = math_sqrt(dx*dx + dy*dy)
    
    local a, b = 0, 0
    if ind then 
        a, b = 45, 89.9 
    else 
        a, b = -10, 45 
    end
    
    local final_ttf = 0
    local c = 0
    
    for step = 1, 11 do
        c = (a + b) * 0.5
        
        local err_a, _ = get_height_error(a, h, dz, v_tick, g_tick, coeff, drag_inv)
        local err_c, ttf_c = get_height_error(c, h, dz, v_tick, g_tick, coeff, drag_inv)
        
        final_ttf = ttf_c
        
        if err_a * err_c < 0 then
            b = c
        else
            a = c
        end
    end
    
    local pitch_deg = (a + b) * 0.5
    local pitch = pitch_deg / 90
    local yaw = (-math_atan(dy, dx) / (math_pi * 2)) + cmps + 0.25

    output.setNumber(1, ((yaw + 0.5) % 1.0) - 0.5)
    output.setNumber(2, pitch)
    output.setNumber(3, final_ttf)
end
