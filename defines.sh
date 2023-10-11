# Replace these 
# DO NOT ADD SPACES
packageName="com.studio23.ss2.InteractionSystem23"
packageDir="Assets/Packages/$packageName"

function find_largest_git_tag() {
    # Run `git tag -l` and capture the output in a variable
    local tags=$(git tag -l)

    # Initialize variables to keep track of the largest tag and its version
    local largest_tag=""
    local largest_version=""

    # Loop through each tag
    while IFS= read -r tag; do
        # Use 'sort -V' to compare version numbers (assumes tags are in a format like 'vX.Y.Z')
        if [[ -z "$largest_version" || "$tag" > "$largest_tag" ]]; then
            largest_tag="$tag"
            largest_version="$tag"
        fi
    done <<< "$tags"

    # Output the largest version tag
    echo "$largest_tag"
}

increment_minor_and_reset_patch() {
    local version="$1"
    local major minor patch

    # Extract the major, minor, and patch version components from the version string
    if [[ "$version" =~ ^v([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
        major="${BASH_REMATCH[1]}"
        minor="${BASH_REMATCH[2]}"
        patch="${BASH_REMATCH[3]}"
    else
        echo "Error: Invalid version format."
        return 1
    fi

    # Increment the minor version and reset the patch version to 0
    ((minor++))
    patch=0

    # Form the new version string
    local new_version="v${major}.${minor}.${patch}"
    echo "$new_version"
}

function find_and_increment_minor_version() {
    local largest_tag=$(find_largest_git_tag)
    local incremented_tag=$(increment_minor_and_reset_patch "$largest_tag")
    echo "$incremented_tag"
}

increment_minor_set_target_version() {
    target_ver=$(find_and_increment_minor_version)
}

function increment_patch_version() {
    local tag="$1"
    # Extract the major, minor, and patch version components from the tag
    if [[ "$tag" =~ ^v([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
        local major="${BASH_REMATCH[1]}"
        local minor="${BASH_REMATCH[2]}"
        local patch="${BASH_REMATCH[3]}"
        # Increment the patch version
        ((patch++))
        # Form the new tag with the incremented patch version
        local new_tag="v${major}.${minor}.${patch}"
        echo "$new_tag"
    else
        # If the tag doesn't match the expected format, return an error message
        echo "Error: Invalid tag format"
    fi
}

function find_and_increment_patch_version() {
    local largest_tag=$(find_largest_git_tag)
    local incremented_tag=$(increment_patch_version "$largest_tag")
    echo "$incremented_tag"
}

increment_patch_set_target_version() {
    target_ver=$(find_and_increment_patch_version)
}

function update_git_repository_with_samples() {
    local package_dir="$1"
    local tag_name="$2"

    # Check if the branch 'upm' exists and delete it if it does
    git branch -d upm &> /dev/null || echo failed

    # Create a new branch 'upm' using git subtree
    git subtree split -P "$package_dir" -b upm
    git checkout upm

    # If the 'Samples' directory exists, rename it and commit the change
    if [[ -f "defines.sh" ]]; then
        rm -f "defines.sh"
    fi

    if [[ -f "updateSamples.sh" ]]; then
        rm -f "updateSamples.sh"
    fi

    if [[ -d "Samples" ]]; then
        git mv Samples Samples~
        rm -f Samples.meta
        git commit -am "fix: Samples => Samples~"
    fi

    # Create a new tag and push it to the 'upm' branch along with tags
    git tag "$tag_name" upm
    git push -u -f origin upm --tags

    # Switch back to the 'main' branch
    git checkout main
}


