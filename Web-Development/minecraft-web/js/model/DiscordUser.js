/**
 * Represents a single Discord user
 */
class DiscordUser{
    profilePicUrl;
    discordId;
    discordName;

    constructor(profilePicUrl, discordId, discordName) {
        this.profilePicUrl = profilePicUrl;
        this.discordId = discordId;
        this.discordName = discordName;
    }
}