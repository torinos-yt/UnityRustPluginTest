mod ear_clipping;
mod test_data;

#[no_mangle]
pub unsafe extern fn triangulate(v : *const f32, size : i32, i : *mut i32) -> bool {
    let points = std::slice::from_raw_parts(v, (size*3) as usize);
    let indices = std::slice::from_raw_parts_mut(i, ((size-2)*3) as usize);

    return ear_clipping::triangulate(points, indices).is_ok();
}

#[cfg(test)]
mod tests {
    use super::*;
    use super::test_data::*;

    #[test]
    fn test() {
        const PTS: [f32;12]= [-0.51, 1.52, 0.,
                               0.57, 1.57, 0.,
                               0.57, 0.38, 0.,
                              -0.49, 0.42, 0.];

        const INDS: [i32;6] = [3,0,1,
                               3,1,2];

        let mut container: [i32;INDS.len()] = Default::default();

        unsafe{ triangulate(&PTS as *const f32, (PTS.len()/3) as i32, &mut container as *mut i32); }
        assert_eq!(container, INDS);
    }

    #[test]
    fn test_large() {
        let mut container: [i32;INDICES.len()] = [0;INDICES.len()];
        unsafe{ triangulate(&POINTS as *const f32, (POINTS.len()/3) as i32, &mut container as *mut i32); }
        assert_eq!(container, INDICES);
    }
}