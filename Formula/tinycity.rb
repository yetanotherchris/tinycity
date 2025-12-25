class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.1.1"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.1.1/tinycity-v3.1.1-osx-arm64"
      sha256 "7e11c6959122d9166cb145edad0efa3780eb67dd84d40c54776199836a716a3c"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.1.1/tinycity-v3.1.1-osx-x64"
      sha256 "055e0384edefc96cf92a731df3a00941a68c99295128640018af4416a5dcd8d5"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.1.1/tinycity-v3.1.1-linux-x64"
    sha256 "b96dcf0027ebc4252b3aae03f3dee0f0be1785838b6efcf1ba2c31a0db7a074f"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.1.1-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.1.1-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.1.1-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end















