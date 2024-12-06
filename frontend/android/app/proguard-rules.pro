# Keep javax annotations
-keep class javax.annotation.** { *; }
-keepattributes *Annotation*

# Keep concurrent annotations
-keep class javax.annotation.concurrent.** { *; }

# Keep lang model elements
-keep class javax.lang.model.element.** { *; }

# Keep other necessary annotations
-keep class com.google.errorprone.annotations.** { *; }
-keep class net.jcip.annotations.** { *; }

# Suppress warnings for javax.annotation
-dontwarn javax.annotation.Nullable

# Suppress warnings for javax.annotation.concurrent
-dontwarn javax.annotation.concurrent.GuardedBy

# Suppress warnings for javax.lang.model.element.Modifier
-dontwarn javax.lang.model.element.Modifier
