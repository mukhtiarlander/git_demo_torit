function ChatItem(chatTemp, memberName, id, created) {
    this.Chat = ko.observable(chatTemp);
    this.MemberName = ko.observable(memberName);
    this.Id = ko.observable(id);
    this.CreatedDisplay = ko.observable("");
    this.MemberProfilePicUrl = ko.observable("");
}