mergeInto(LibraryManager.library, {
  HandleRequestData: function () {
    window.dispatchReactUnityEvent(
      "HandleRequestData"
    );
  },
});