/**
 * Custom commit analyzer for semantic-release.
 *
 * - On prerelease branches (e.g. develop): always triggers a patch release,
 *   so every push produces a new beta version (e.g. 1.1.0-beta.4 -> 1.1.0-beta.5).
 * - On release branches (e.g. main): falls back to conventional commits rules.
 */
module.exports = {
  analyzeCommits: async (pluginConfig, context) => {
    const { branch, commits } = context;

    // Prerelease branches (develop): always bump the version
    if (branch.type === "prerelease") {
      return "patch";
    }

    // Release branches (main): use conventional commits rules
    for (const commit of commits) {
      if (commit.message.includes("BREAKING CHANGE")) {
        return "major";
      }
      const match = commit.message.match(/^(\w+)(\(.*\))?!?:/);
      if (match) {
        const type = match[1];
        if (type === "feat") return "minor";
        if (type === "fix" || type === "perf") return "patch";
      }
    }
    return null;
  },
};
