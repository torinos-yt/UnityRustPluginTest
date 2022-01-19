mod vector;
use vector::Vector;

pub fn triangulate(points: &[f32], indices: &mut [i32]) -> Result<(), String> {
    return EarClipper::new(points).ear_clipping(points, indices);
}

struct EarClipper {
    indices: Vec<usize>,
    ear_tips: Vec<usize>
}

// original : kaiware007
// https://github.com/IndieVisualLab/UnityGraphicsProgramming4/tree/master/Assets/TriangulationByEarClipping
impl EarClipper {
    fn new(points: &[f32]) -> EarClipper {
        let count = points.len() / 3;
        let ear_tips: Vec<usize> = Vec::new();

        let mut indices: Vec<usize> = Vec::with_capacity(count);
        for i in 0..count { indices.push(i); }

        let mut clipper = EarClipper { indices, ear_tips };
        for i in 0..count {
            clipper.check_vertex(points, i)
        }

        return clipper;
    }

    fn ear_clipping(&mut self, points: &[f32], indices: &mut [i32]) -> Result<(), String> {
        let mut triangle_index = 0usize;

        while !self.ear_tips.is_empty() {
            let now_index = self.ear_tips[0];

            let ii = self.indices.iter().position(|&x| x == now_index).unwrap();
            let count = self.indices.len();
            if ii < count {
                let prev_index = if ii == 0       {*self.indices.last().unwrap() } else {self.indices[ii-1]};
                let next_index = if ii == count-1 {*self.indices.first().unwrap()} else {self.indices[ii+1]};

                indices[triangle_index + 0] = prev_index as i32;
                indices[triangle_index + 1] = now_index  as i32;
                indices[triangle_index + 2] = next_index as i32;

                triangle_index += 3;

                if self.indices.len() == 3 { break; }

                self.ear_tips.remove(0);

                let rm_index = self.indices.iter().position(|&x| x == now_index).unwrap();
                self.indices.remove(rm_index);

                let both_list = [prev_index, next_index];
                for i in both_list {
                    let node_index = self.indices.iter().position(|&x| x == i).unwrap();
                    self.check_vertex(points, node_index);
                }
            }else {
                return Err(String::from("index not found"));
            }
        }

        return Ok(());
    }


    fn check_vertex(&mut self, points: &[f32], index : usize) {
        let size = self.indices.len();

        let prev_index = if index == 0      {*self.indices.last().unwrap() } else {self.indices[index-1]};
        let next_index = if index == size-1 {*self.indices.first().unwrap()} else {self.indices[index+1]};
        let now_index = self.indices[index];

        let prev_vertex = Vector {
            x: points[prev_index * 3 + 0],
            y: points[prev_index * 3 + 1],
            z: points[prev_index * 3 + 2],
        };
        let next_vertex = Vector {
            x: points[next_index * 3 + 0],
            y: points[next_index * 3 + 1],
            z: points[next_index * 3 + 2],
        };
        let now_vertex = Vector {
            x: points[now_index * 3 + 0],
            y: points[now_index * 3 + 1],
            z: points[now_index * 3 + 2],
        };

        if is_angle_less_pi(&now_vertex, &prev_vertex, &next_vertex) {
            let mut is_ear = true;

            for i in 0..size {
                let index = self.indices[i];
                if index==prev_index || index==now_index || index==next_index {continue;}

                let p = Vector{
                    x: points[index * 3 + 0],
                    y: points[index * 3 + 1],
                    z: points[index * 3 + 2],
                };

                if Vector::distance(&p, &prev_vertex) <= f32::EPSILON {continue;}
                if Vector::distance(&p, &next_vertex) <= f32::EPSILON {continue;}
                if Vector::distance(&p, &now_vertex ) <= f32::EPSILON {continue;}

                if is_in_triangle(&p, &prev_vertex, &now_vertex, &next_vertex) <= 0 {
                    is_ear = false;
                    break;
                }
            }

            if is_ear {
                if !self.ear_tips.contains(&now_index) {
                    self.ear_tips.push(now_index);
                }
            }else {
                if self.ear_tips.contains(&now_index) {
                    let rm_index = self.ear_tips.iter().position(|&x| x == now_index).unwrap();
                    self.ear_tips.remove(rm_index);
                }
            }
        }
    }
}

#[inline(always)]
fn check_line(o: &Vector, p1: &Vector, p2: &Vector) -> i32 {
    let x0 = (o.x - p1.x)  as f64;
    let y0 = (o.y - p1.y)  as f64;
    let x1 = (p2.x - p1.x) as f64;
    let y1 = (p2.y - p1.y) as f64;
    let x0y1 = x0 * y1;
    let x1y0 = x1 * y0;
    let det = x0y1 - x1y0;

    return if det > 0f64 {1} else if det < 0f64 {-1} else {0};
}

#[inline(always)]
fn is_angle_less_pi(o: &Vector, a: &Vector, b: &Vector) -> bool {
    return ( (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x) ) > 0.;
}

fn is_in_triangle(o: &Vector, p1: &Vector, p2: &Vector, p3: &Vector) -> i32 {
    let sign1 = check_line(o, p2, p3);
    if sign1 < 0
    {
        return 1;
    }

    let sign2 = check_line(o, p3, p1);
    if sign2 < 0
    {
        return 1;
    }

    let sign3 = check_line(o, p1, p2);
    if sign3 < 0
    {
        return 1;
    }

    return if (sign1 != 0) && (sign2 != 0) && (sign3 != 0) {-1} else {0};
}
