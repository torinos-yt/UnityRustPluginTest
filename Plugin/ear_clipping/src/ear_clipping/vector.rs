pub struct Vector {
    pub x: f32,
    pub y: f32,
    pub z: f32,
}

impl Vector {
    pub fn distance(v1: &Vector, v2: &Vector) -> f32 {
        let px = (v1.x - v2.x).powi(2);
        let py = (v1.y - v2.y).powi(2);
        let pz = (v1.z - v2.z).powi(2);

        return (px + py + pz).sqrt();
    }
}
