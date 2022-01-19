#[no_mangle]
pub unsafe extern fn test_add(a : i32, b : i32) -> i32 {
    return  a + b;
}