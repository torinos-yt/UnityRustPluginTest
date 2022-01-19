echo "build library : plubin_test"
cd `dirname $0`
cargo build --target x86_64-pc-windows-gnu --release
echo "copy binary"
cp target/x86_64-pc-windows-gnu/release/plugin_test.dll ../../Assets/Plugin/Windows/
