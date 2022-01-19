echo "build library : ear_clipping"
cd `dirname $0`
cargo build --target x86_64-pc-windows-gnu --release
echo "copy binary"
cp target/x86_64-pc-windows-gnu/release/ear_clipping.dll ../../Assets/Plugin/Windows/
