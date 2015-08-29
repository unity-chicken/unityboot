# unityboot

Spring Boot makes it easy to create Unity based Applications that you can "just make your game".

### Features
> Services
* Service class: Service root class
* sb: string bundle service with multi-language support
* ...
>

> Utils
* ...
>

> Editor Scripts
* Builder: You can make iOS / Android asset bundles
>

### Initialize Services

```c#
public class IntroSceneController : SceneControllerBase {
    Logger logger = new Logger("IntroSceneController");

    void Awake() {
        InitController();
    }

    IEnumerator Start() {
        yield return StartCoroutine(InitializeService());
    }

    IEnumerator InitializeService() {
        yield return Service.Run(Service.sb.Initialize("en", "ko"));
        yield return Service.Run(Service.setting.Initialize());
        Service.ready = true;

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Service.scene.LoadLevel("Battle"));
    }
}
```
