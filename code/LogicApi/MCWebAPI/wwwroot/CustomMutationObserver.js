class CustomMutationObserver {

    mutationListener;

    targetNode;
    addedListener;
    removedListener;

    constructor(targetNode, addedListener, removedListener) {
        this.targetNode = targetNode;
        this.addedListener = addedListener;
        this.removedListener = removedListener;
    }

    start() {
        if (this.mutationListener != null)
            return;

        const callback = (mutationList, observer) => {
            for (const mutation of mutationList) {

                if (typeof this.addedListener === 'function') {
                    for (const mutationNode of mutation.addedNodes) {
                        this.addedListener(mutationNode);
                    } 
                }

                if (typeof this.removedListener === 'function') {
                    for (const removedNode of mutation.removedNodes) {
                        this.removedListener(removedNode)
                    }
                }
            }
        };
        
        this.mutationListener = new MutationObserver(callback);


        this.mutationListener.observe(this.targetNode, { childList: true });
    }


    close() {
        this.mutationListener.disconnect();
    }
}