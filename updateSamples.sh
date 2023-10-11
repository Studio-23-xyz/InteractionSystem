source ./defines.sh

if [ "$#" -ne 1 ]; then
    echo "Error: This script requires a single argument (either 'patch' or 'minor')."
    exit 1
fi

case "$1" in
    "patch")
        increment_patch_set_target_version
        ;;
    "minor")
        increment_minor_set_target_version
        ;;
    *)
        echo "Error: Invalid argument. Please use 'patch' or 'minor'."
        exit 1
        ;;
esac

echo "pushing ver $target_ver" 
update_git_repository_with_samples "$packageName" "$packageDir"