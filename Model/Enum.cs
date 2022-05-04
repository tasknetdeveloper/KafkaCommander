
namespace Model
{
    public enum Command { 
        empty,
        getDrive,
        getDir,
        getFiles,
        copyDir,
        copyFile,
        mkDir,
        deleteFile,
        deleteDir,
        cd,
        readFile,
    }

    public enum KafkaType { 
        get,
        send,
        cancel
    }

}
