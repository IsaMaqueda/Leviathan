(module
  (import "kaida" "max" (func $max (param i32) (param i32) (result i32)))
  (func
    (export "start")
    (result i32)
    i32.const 1
    i32.const 4
    i32.const 2 
     i32.mul 
    i32.const 2 
     i32.mul 
    i32.const 4
    call $max 
    i32.const -4
    i32.const 2 
     i32.mul 
    call $max 
     i32.add
    i32.const -3
     i32.add
    i32.const 10
    i32.const 2 
     i32.mul 
    call $max 
  )
)
